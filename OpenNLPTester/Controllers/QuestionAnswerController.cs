using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonServiceLocator;
using QuestionAnswerAi.Models;
using QuestionAnswerAi.Solr;
using QuestionAnswerAi.Solr.Models;
using QuestionAnswerAi.Utils;
using SolrNet;

namespace QuestionAnswerAi.Controllers
{
    /// <summary>
    /// This is the class we init in the Web API so all dependencies of all used methods need to be
    /// created at the init of this controller.
    /// </summary>
    public class QuestionAnswerController
    {
        private ISolrOperations<QaSettingsModel> _qaSettings;
        private SolrQueryMethods<QaSettingsModel> _qaSettingsMethods;
        private ISolrOperations<WikiModelResult> _solr;
        private SolrQueryMethods<WikiModelResult> _solrMethods;
        private OpenNLPUtils openNlpUtil;
        private SolrQueryResults<QaSettingsModel> getSettings;
        private SolrQueryResults<WikiModelResult> resultList;
        private OpenNLPMethods nlpMethods;
        private AnswerDeterminer pAnswer;
        private QuestionParser qparse;
        private PossibleResultsModel foundAnswer;

        public QuestionAnswerController(string modelsPath) {

            // get instance of the QaSettingsModel collection created at startup and pass it into the
            // SolrQueryMethods so that we can create our Dictionary collection.
            _qaSettings = ServiceLocator.Current.GetInstance<ISolrOperations<QaSettingsModel>>();
            _qaSettingsMethods = new SolrQueryMethods<QaSettingsModel>(_qaSettings);

            // get instance of the WikiModelResult collection created at startup and pass it into the
            // SolrQueryMethods so that we can do Queries on the wikipedia collection.
            _solr = ServiceLocator.Current.GetInstance<ISolrOperations<WikiModelResult>>();
            _solrMethods = new SolrQueryMethods<WikiModelResult>(_solr);

            openNlpUtil = new OpenNLPUtils();

            // get models path from startup instance so that we can use our OpenNlpMethods and other dependencies
            nlpMethods = new OpenNLPMethods(modelsPath);
            pAnswer = new AnswerDeterminer(nlpMethods, openNlpUtil);
            qparse = new QuestionParser();
        }


        /// <summary>
        /// Takes a sentence from an end user, parses and tries to find an answer in the Solr Database.
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public QuestionAnswerModel GetAnswerFromQuestion(string sentence) 
        {
            // create an instance of the return model.
            QuestionAnswerModel answerModel = new QuestionAnswerModel();
            // wrap entire block in a try to bubble up the exception to the web api
            try
            {
                // get the known question determinations from the Solr Database.
                getSettings = _qaSettingsMethods.QueryAll();
                // create the dictionary for the Question determinations.
                openNlpUtil.QuestionDetermination = getSettings.ToDictionary(
                    settingResults => settingResults.QuestionIdentifier,
                    settingResults => settingResults.NerTypes.ToArray());

                // parse the question using OpenNlp's parser
                QuestionParserResponseModel parseQuestionObj = qparse.parseQuestion(sentence);
                answerModel.TrainingModel.OriginalQuestion = parseQuestionObj.Question;
                answerModel.TrainingModel.QuestionIdentifier = parseQuestionObj.QuestionIdentifier;

                // if we don't have a definition for the Question Identifier we return that they need to tell us what it means
                // and add it to the dictionary and try again.
                if (!openNlpUtil.QuestionDetermination.ContainsKey(parseQuestionObj.QuestionIdentifier))
                {
                    answerModel.StatusCode = "NEEDS_HUMAN_TRAINING";
                    answerModel.StatusMessage =
                        "The definition doesn't exist in the Question Identifier, Call DoExternalHumanTraining with the TrainingModel that was returned";
                    answerModel.NeedsHumanTraining = true;
                    return answerModel;
                }

                // we look up the broken up query in the Solr database with the list of results from the OpenNlp methods
                // ex.
                //   Who is the president of the united states of america?
                //   we would query:
                //        president
                //        united states of america
                resultList = _solrMethods.QueryList(parseQuestionObj, "revisionText");
                // get list of answer and pass for the original QuestionParserResponseModel and the results of
                // the query to try to find and score the answers.
                // This will return the highest score answer to the user.
                foundAnswer = pAnswer.TryFindAnswer(resultList, parseQuestionObj);

                // if we find an answer we return a SUCCESS to the user with the result.
                if (foundAnswer != null)
                {
                    answerModel.StatusCode = "SUCCESS";
                    answerModel.StatusMessage = "We found an answer to the question.";
                    answerModel.Score = foundAnswer.Score;
                    answerModel.Answer = foundAnswer.FoundNerResult;
                    answerModel.FoundAnswerSentence = foundAnswer.PossibleResultSentence;
                    return answerModel;
                }
                // If we don't find an answer we let them know that our system couldn't find it.
                answerModel.StatusCode = "NO_ANSWER_FOUND";
                answerModel.StatusMessage = "We did not find an answer to this question";
                return answerModel;
            }
            catch (Exception e)
            {
                answerModel.StatusCode = "ERROR";
                answerModel.StatusMessage = e.Message;
                return answerModel;
            }
        }

        /// <summary>
        /// This is a method used by the API to add definitions to questions that our system may not have encountered yet.
        /// The model that must be returned is passed to the front end when notified.
        /// </summary>
        /// <param name="humanTrainingModel"></param>
        /// <returns></returns>
        public ExternalHumanTrainingResponseModel DoExternalHumanTraining(ExternalHumanTrainingModel humanTrainingModel) {

            // create a settings model to pass to the SOLR database to update a unknown question identifier definition.
            QaSettingsModel newDictonarySetting = new QaSettingsModel();
            // create a list of possible definitions
            List<string> tempList = new List<string>();

            // add the QuestionIdentifier to the newDictionarySetting prior to checking definitions.
            newDictonarySetting.QuestionIdentifier = humanTrainingModel.QuestionIdentifier;

            // create the list of types that will be added as definitions.
            if (humanTrainingModel.Date) {
                tempList.Add("date");
            }
            if (humanTrainingModel.Location) {
                tempList.Add("location");
            }
            if (humanTrainingModel.Money)
            {
                tempList.Add("money");
            }
            if (humanTrainingModel.Organization)
            {
                tempList.Add("organization");
            }
            if (humanTrainingModel.Percentage)
            {
                tempList.Add("percentage");
            }
            if (humanTrainingModel.Person)
            {
                tempList.Add("person");
            }
            if (humanTrainingModel.Time)
            {
                tempList.Add("time");
            }
            newDictonarySetting.NerTypes = tempList;
            // add the entry to the database.
            _qaSettingsMethods.AddSingleEntry(newDictonarySetting);

            // return a model with the original question to the end user so that they can pass it back to the
            // GetAnswerFromQuestion method.
            ExternalHumanTrainingResponseModel response = new ExternalHumanTrainingResponseModel
            {
                StatusCode = "SUCCESS",
                StatusMessage = "We have successfully updated the collection with the unknown identifier",
                OriginalQuestion = humanTrainingModel.OriginalQuestion
            };
            return response;
        }
    }
}
