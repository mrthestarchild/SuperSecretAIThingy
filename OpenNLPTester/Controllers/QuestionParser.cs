using OpenNLP.Tools.Chunker;
using OpenNLPTester.Models;
using OpenNLPTester.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLPTester.Controllers
{
    class QuestionParser
    {
        private QuestionParserResponseModel qParserResponse = new QuestionParserResponseModel();
        private OpenNLPMethods openNlpMethods = new OpenNLPMethods(Directory.GetCurrentDirectory() + @"\Resources\Models\");
        private OpenNLPUtils openNLPUtils = new OpenNLPUtils();

        /// <summary>
        /// Parses question from end user to be passed into SolrQuery
        /// </summary>
        /// <param name="question"></param>
        /// <returns>QuestionParserResponseModel</returns>
        public QuestionParserResponseModel parseQuestion(string question)
        {
            string[] modelTypes = { "date" };
            // set the initial question to reference if needed later
            qParserResponse.Question = question;

            // check for time reference in question to know if we need to look for now or a specific date
            string initQNERCheck = openNlpMethods.NER(question, modelTypes);
            qParserResponse.HasTimeRef = initQNERCheck.ToLower().Contains("<date>") ? true : false;

            // get the chunked version of the question
            List<SentenceChunk> chunkedQuestion = openNlpMethods.Chunker(question);
            qParserResponse.QueryParams = new List<string>();

            //get noun phrases
            chunkedQuestion.ForEach(cq =>
            {
                foreach (var value in openNLPUtils.ChunkerTags) {
                    if (cq.Tag == value.Key) {
                        if (cq.Tag == "NP") {
                            var getNPString = "";
                            cq.TaggedWords.ForEach(chunk =>
                            {
                                getNPString += chunk.Tag != "DT" ? chunk.Word.ToLower() + " " : "";

                            });
                            if (getNPString.Length > 0) {
                                getNPString.Substring(0, getNPString.Length - 2);

                                cq.TaggedWords.ForEach(chunkQuestionIdentity =>
                                {
                                    if (chunkQuestionIdentity.Tag == "WP") {
                                        qParserResponse.QuestionIdentifier = getNPString.Trim();
                                    }
                                });
                                qParserResponse.QueryParams.Add(getNPString.Trim());
                            }
                        }
                    }
                }
                for (int x = 0; x < qParserResponse.QueryParams.Count; x++) {
                    if (qParserResponse.QuestionIdentifier == qParserResponse.QueryParams[x]) {
                        qParserResponse.QueryParams.RemoveAt(x);
                    }
                }
            });
            return qParserResponse;
        } 
    }
}
