using QuestionAnswerAi.Models;
using QuestionAnswerAi.Solr.Models;
using QuestionAnswerAi.Utils;
using SolrNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace QuestionAnswerAi.Controllers
{
    // TODO make generic
    class AnswerDeterminer
    {
        OpenNLPMethods _methods;
        OpenNLPUtils _openNlpUtil;
        List<string> possibleAnswers = new List<string>();

        public AnswerDeterminer(OpenNLPMethods methods, OpenNLPUtils utils)
        {
            _methods = methods;
            _openNlpUtil = utils;
        }

        /// <summary>
        /// Returns most likely answer found by NER
        /// </summary>
        /// <param name="resultsList"></param>
        /// <param name="parseQuestionObj"></param>
        /// <returns></returns>
        public PossibleResultsModel TryFindAnswer(SolrQueryResults<WikiModelResult> resultsList, QuestionParserResponseModel parseQuestionObj)
        {
            string[] nerTypes = GetNERTypes(parseQuestionObj);
            var isfound = false;
            List<PossibleResultsModel> answer = new List<PossibleResultsModel>();
            resultsList.ForEach(sortedResult =>
            {
                if (isfound) return;
                // TODO make generic
                string[] splitSentence = _methods.SentenceSplitter(sortedResult.RevisionText.First());
                
                var possibleAnswerList = GetPossibleResultItems(parseQuestionObj, nerTypes, splitSentence, sortedResult.Score);
                var scoredList = ScoreAnswerByIndexLookup(possibleAnswerList, parseQuestionObj);
                var list = DetermineAnswerByScore(scoredList);
                if (list != null) answer.Add(list);
            });
            // return the answer list for testing. 
            return DetermineAnswerByScore(answer);
        }

        /// <summary>
        /// Gets array of string paramaters to use as a look up in GetPossibleResults
        /// </summary>
        /// <param name="qprm"></param>
        /// <returns></returns>
        private string[] GetNERTypes(QuestionParserResponseModel qprm)
        {
            if (_openNlpUtil.QuestionDetermination.TryGetValue(qprm.QuestionIdentifier, out string[] value)) {
                return value;
            }
            return null;
        }

        /// <summary>
        /// Gets Most likely result based on index of the query params.
        /// </summary>
        /// <param name="possibleAnswers"></param>
        /// <param name="parseQuestionObj"></param>
        /// <returns></returns>
        private List<PossibleResultsModel> ScoreAnswerByIndexLookup(List<PossibleResultsModel> possibleAnswers, QuestionParserResponseModel parseQuestionObj)
        {
            Regex whitespaceCheck = new Regex("[ ]{1,}");
            foreach (var result in possibleAnswers)
            {
                var queryParamLength = parseQuestionObj.QueryParams.Count;
                foreach (var queryParam in parseQuestionObj.QueryParams)
                {
                    List<ScoringModel> queryScoringList = new List<ScoringModel>();
                    int numParamFound = result.NerResultSentence.Split(new string[] { queryParam }, StringSplitOptions.None).Length - 1;
                    var startingIndex = 0;
                    for (var x = 0; x < numParamFound; x++)
                    {
                        ScoringModel scoringModel = new ScoringModel();
                        scoringModel.StartIndex = result.NerResultSentence.IndexOf(queryParam, startingIndex);
                        scoringModel.EndIndex = queryParam.Length + scoringModel.StartIndex;
                        int substringLength = 0;
                        int substringStartIndex = 0;
                        if (result.StartIndex > scoringModel.StartIndex)
                        {
                            substringLength = result.StartIndex - scoringModel.StartIndex;
                            substringStartIndex = scoringModel.StartIndex;
                        }
                        else
                        {
                            substringLength = scoringModel.EndIndex - result.EndIndex;
                            substringStartIndex = result.EndIndex;
                            // do boosting here because the name is followed by the discription
                            scoringModel.Boost += 1;

                        }
                        if (substringStartIndex < 0) substringStartIndex = 0;
                        if (substringLength < 0) substringLength = 0;
                        startingIndex = scoringModel.EndIndex + queryParam.Length;
                        scoringModel.SubstringedSentence = result.NerResultSentence.Substring(substringStartIndex, substringLength);
                        scoringModel.WordDistance = whitespaceCheck.Split(scoringModel.SubstringedSentence).Length - 1;
                        queryScoringList.Add(scoringModel);
                    }
                    if (queryScoringList.Count > 0)
                    {
                        var bestModel = GetBestScoreFromQueryModel(queryScoringList);
                        // if no param is found we remove 1 from score.
                        var noParamFound = (result.PossibleResultSentence.ToLower().Contains(queryParam.ToLower())) ? 0 : -2.5;
                        result.Score = (result.Score + (-bestModel.WordDistance * .1)) + noParamFound + bestModel.Boost;
                    }
                    else
                    {
                        result.Score -= 5;
                    }
                    queryParamLength--;
                }
            }
            return possibleAnswers;
        }

        private ScoringModel GetBestScoreFromQueryModel(List<ScoringModel> models)
        {
            int lowest = 99999;
            int length = models.Count;
            for (var x = 0; x < length; x++)
            {
                if (models[x].WordDistance < lowest)
                {
                    lowest = models[x].WordDistance;
                }
            }
            return models.Find(model => model.WordDistance == lowest);
        }

        // TODO: implement this after testing. Figure out if the answer is correct first (solr query to title?).
        private PossibleResultsModel DetermineAnswerByScore(List<PossibleResultsModel> possibleAnswers)
        {
            double highest = -99999;
            int length = possibleAnswers.Count;
            for (var x = 0; x < length; x++)
            {
                if (possibleAnswers[x].Score > highest)
                {
                    highest = possibleAnswers[x].Score;
                }
            }
            return possibleAnswers.Find(model => model.Score == highest);
        }

        /// <summary>
        /// Gets List of possible results as a model based on NER type.
        /// </summary>
        /// <param name="parseQuestionObj"></param>
        /// <param name="nerTypes"></param>
        /// <param name="splitSentence"></param>
        /// <returns></returns>
        private List<PossibleResultsModel> GetPossibleResultItems(QuestionParserResponseModel parseQuestionObj, string[] nerTypes, string[] splitSentence, double score)
        {
            // TODO: return Object with different possible answers
            var indexes = new List<PossibleResultsModel>();
            foreach (var spSentence in splitSentence) {
                // if (isfound) break;
                foreach (var queryParams in parseQuestionObj.QueryParams) {
                    if (spSentence.ToLower().Contains(queryParams)) {
                        if (parseQuestionObj.HasTimeRef) {
                            var nerSentence = _methods.NER(spSentence, nerTypes);
                            foreach (var type in nerTypes) {
                                int numNerFound = nerSentence.Split(new string[] { $"<{type}>" }, StringSplitOptions.None).Length - 1;
                                var startingIndex = 0;
                                for (int x = 0; x < numNerFound; x++) {
                                    var possibleResults = new PossibleResultsModel();
                                    possibleResults.StartIndex = nerSentence.IndexOf($"<{type}>", startingIndex) + $"<{type}>".Length;
                                    possibleResults.EndIndex = nerSentence.IndexOf($"</{type}>", possibleResults.StartIndex);
                                    var substringLength = possibleResults.EndIndex - possibleResults.StartIndex;
                                    startingIndex = possibleResults.EndIndex + $"<{type}>".Length;
                                    possibleResults.FoundNerResult = nerSentence.Substring(possibleResults.StartIndex, substringLength);
                                    possibleResults.PossibleResultSentence = spSentence.ToLower();
                                    possibleResults.NerResultSentence = nerSentence.ToLower();
                                    possibleResults.Score = score;
                                    indexes.Add(possibleResults);
                                }
                            }
                        }
                        else {
                            string[] currentTimeFrame = _openNlpUtil.NoTimeFrameDefs;
                            foreach (var time in currentTimeFrame) {
                                if (spSentence.ToLower().Contains(time)) {
                                    var nerSentence = _methods.NER(spSentence, nerTypes);
                                    foreach (var type in nerTypes) {
                                        // TODO: break this out into a method of its own
                                        int numNerFound = nerSentence.Split(new string[] { $"<{type}>" }, StringSplitOptions.None).Length - 1;
                                        var startingIndex = 0;
                                        for (int x = 0; x < numNerFound; x++)
                                        {
                                            var possibleResults = new PossibleResultsModel();
                                            possibleResults.StartIndex = nerSentence.IndexOf($"<{type}>", startingIndex) + $"<{type}>".Length ;
                                            possibleResults.EndIndex = nerSentence.IndexOf($"</{type}>", possibleResults.StartIndex);
                                            var substringLength = possibleResults.EndIndex - possibleResults.StartIndex;
                                            startingIndex = possibleResults.EndIndex + $"<{type}>".Length;
                                            possibleResults.FoundNerResult = nerSentence.Substring(possibleResults.StartIndex, substringLength);
                                            possibleResults.PossibleResultSentence = spSentence.ToLower();
                                            possibleResults.NerResultSentence = nerSentence.ToLower();
                                            possibleResults.Score = score;
                                            indexes.Add(possibleResults);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return indexes;
        }
    }
}
