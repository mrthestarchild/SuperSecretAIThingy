using OpenNLPTester.Models;
using QuestionAnswerAi.Models;
using QuestionAnswerAi.Solr.Models;
using QuestionAnswerAi.Utils;
using SolrNet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestionAnswerAi.Controllers
{
    // TODO make generic
    class AnswerDeterminer
    {
        OpenNLPMethods methods;
        OpenNLPUtils openNlpUtil;
        List<string> possibleAnswers = new List<string>();

        public AnswerDeterminer(OpenNLPMethods _methods, OpenNLPUtils _utils)
        {
            methods = _methods;
            openNlpUtil = _utils;
        }

        /// <summary>
        /// Returns most likely answer
        /// </summary>
        /// <param name="resultsList"></param>
        /// <param name="parseQuestionObj"></param>
        /// <returns></returns>
        public List<string> TryFindAnswer(SolrQueryResults<WikiModelResult> resultsList, QuestionParserResponseModel parseQuestionObj)
        {
            string[] nerTypes = GetNERTypes(parseQuestionObj);
            var isfound = false;
            resultsList.ForEach(sortedResult =>
            {
                if (isfound) return;
                // TODO make generic
                string[] splitSentence = methods.SentenceSplitter(sortedResult.RevisionText.First());
                
                var possibleAnswerList = GetPossibleResultItems(parseQuestionObj, nerTypes, splitSentence);
                var answer = DetermineAnswerByIndexLookup(possibleAnswerList, parseQuestionObj); 
            });
            return possibleAnswers;
        }

        /// <summary>
        /// Gets array of string paramaters to use as a look up in GetPossibleResults
        /// </summary>
        /// <param name="qprm"></param>
        /// <returns></returns>
        private string[] GetNERTypes(QuestionParserResponseModel qprm)
        {
            if (openNlpUtil.QuestionDetermination.TryGetValue(qprm.QuestionIdentifier, out string[] value)) {
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
        private string DetermineAnswerByIndexLookup(List<PossibleResultsModel> possibleAnswers, QuestionParserResponseModel parseQuestionObj)
        {
            foreach (var queryParams in parseQuestionObj.QueryParams) {

            }
            return "";
        }

        /// <summary>
        /// Gets List of possible results based on NER type.
        /// </summary>
        /// <param name="parseQuestionObj"></param>
        /// <param name="nerTypes"></param>
        /// <param name="splitSentence"></param>
        /// <returns></returns>
        private List<PossibleResultsModel> GetPossibleResultItems(QuestionParserResponseModel parseQuestionObj, string[] nerTypes, string[] splitSentence)
        {
            // TODO: return Object with different possible answers
            var indexes = new List<PossibleResultsModel>();
            foreach (var spSentence in splitSentence) {
                // if (isfound) break;
                foreach (var queryParams in parseQuestionObj.QueryParams) {
                    if (spSentence.ToLower().Contains(queryParams)) {
                        if (parseQuestionObj.HasTimeRef) {
                            var nerSentence = methods.NER(spSentence, nerTypes);
                            foreach (var type in nerTypes) {
                                int numNerFound = nerSentence.Split(new string[] { $"<{type}>" }, StringSplitOptions.None).Length - 1;
                                for (int x = 0; x < numNerFound; x++) {
                                    var possibleResults = new PossibleResultsModel();
                                    possibleResults.StartIndex = nerSentence.IndexOf($"<{type}>") + $"<{type}>".Length;
                                    possibleResults.EndIndex = (nerSentence.IndexOf($"</{type}>") - ((nerSentence.IndexOf($"<{type}>") + $"<{type}>".Length)));
                                    possibleResults.NerResult = nerSentence.Substring(possibleResults.StartIndex, possibleResults.EndIndex);
                                    possibleResults.PossibleResult = spSentence;
                                    indexes.Add(possibleResults);
                                }
                            }
                        }
                        else {
                            string[] currentTimeFrame = openNlpUtil.NoTimeFrameDefs;
                            foreach (var time in currentTimeFrame) {
                                if (spSentence.ToLower().Contains(time)) {
                                    var nerSentence = methods.NER(spSentence, nerTypes);
                                    foreach (var type in nerTypes) {
                                        int numNerFound = nerSentence.Split(new string[] { $"<{type}>" }, StringSplitOptions.None).Length - 1;
                                        for (int x = 0; x < numNerFound; x++)
                                        {
                                            var possibleResults = new PossibleResultsModel();
                                            possibleResults.StartIndex = nerSentence.IndexOf($"<{type}>") + $"<{type}>".Length;
                                            possibleResults.EndIndex = (nerSentence.IndexOf($"</{type}>") - ((nerSentence.IndexOf($"<{type}>") + $"<{type}>".Length)));
                                            possibleResults.NerResult = nerSentence.Substring(possibleResults.StartIndex, possibleResults.EndIndex);
                                            possibleResults.PossibleResult = spSentence;
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
