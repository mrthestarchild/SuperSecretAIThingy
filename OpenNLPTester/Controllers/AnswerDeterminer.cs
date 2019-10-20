using QuestionAnswerAi.Models;
using QuestionAnswerAi.Solr.Models;
using QuestionAnswerAi.Utils;
using SolrNet;
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
        public List<string> tryFindAnswer(SolrQueryResults<WikiModelResult> resultsList, QuestionParserResponseModel parseQuestionObj)
        {
            string[] nerTypes = getNERTypes(parseQuestionObj);
            var isfound = false;
            resultsList.ForEach(sortedResult =>
            {
                if (isfound) return;
                // TODO make generic
                string[] splitter = methods.SentenceSplitter(sortedResult.RevisionText.First());
                foreach (var spSentence in splitter)
                {
                    if (isfound) break;
                    foreach (var queryParams in parseQuestionObj.QueryParams)
                    {
                        if (isfound) break;
                        if (spSentence.ToLower().Contains(queryParams))
                        {
                            if (parseQuestionObj.HasTimeRef)
                            {
                                var tester = methods.NER(spSentence, nerTypes);
                                foreach (var type in nerTypes)
                                {
                                    if (tester.ToLower().Contains($"<{type}>"))
                                    {
                                        //possibleAnswers.Add(tester.Substring((tester.IndexOf($"<{type}>") + $"<{type}>".Length), (tester.IndexOf($"</{type}>") - ((tester.IndexOf($"<{type}>") + $"<{type}>".Length)))));
                                        isfound = true;
                                        possibleAnswers.Add(spSentence);
                                    }
                                }
                            }
                            else
                            {
                                string[] currentTimeFrame = openNlpUtil.NoTimeFrameDefs;
                                foreach (var time in currentTimeFrame)
                                {
                                    if (spSentence.ToLower().Contains(time))
                                    {
                                        var tester = methods.NER(spSentence, nerTypes);
                                        foreach (var type in nerTypes)
                                        {
                                            if (tester.ToLower().Contains($"<{type}>"))
                                            {
                                                //possibleAnswers.Add(tester.Substring((tester.IndexOf($"<{type}>") + $"<{type}>".Length), (tester.IndexOf($"</{type}>") - ((tester.IndexOf($"<{type}>") + $"<{type}>".Length)))));
                                                isfound = true;
                                                possibleAnswers.Add(spSentence);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            });
            return possibleAnswers;
        }

        private string[] getNERTypes(QuestionParserResponseModel qprm)
        {
            if (openNlpUtil.QuestionDetermination.TryGetValue(qprm.QuestionIdentifier, out string[] value))
            {
                return value;
            }
            return null;
        }
    }
}
