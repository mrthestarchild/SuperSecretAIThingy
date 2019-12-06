using QuestionAnswerAi.Models;
using QuestionAnswerAi.Solr;
using QuestionAnswerAi.Solr.Models;
using System;
using System.Collections.Generic;

namespace QuestionAnswerAi.Controllers
{
    class HumanTrainerAssistant
    {
        private OpenNLPMethods _nlpMethods;
        private string _modelsPath;
        private SolrQueryMethods<QaSettingsModel> _qaSettingsMethods;

        public HumanTrainerAssistant(OpenNLPMethods nlpMethods, SolrQueryMethods<QaSettingsModel> qaSettingsMethods, string modelsPath)
        {
            _nlpMethods = nlpMethods;
            _qaSettingsMethods = qaSettingsMethods;
            _modelsPath = modelsPath;
        }
        public void AskForDefinition(QuestionParserResponseModel parseQuestionObj)
        {
            // TODO: break this out into a human trainer class
            Console.WriteLine($"I don't know what \"{parseQuestionObj.QuestionIdentifier}\" means.");
            var nerModelTypes = _nlpMethods.GetAllModelTypes($@"{_modelsPath}NameFind\");
            var parsedResult = -1;
            var userInput = "";
            List<string> results = new List<string>();
            while (parsedResult != nerModelTypes.Length)
            {
                Console.WriteLine($"Which one of these does \"{parseQuestionObj.QuestionIdentifier}\" mean? (choose one selection at a time):");
                for (var x = 0; x <= nerModelTypes.Length; x++)
                {
                    if (x == nerModelTypes.Length)
                    {
                        Console.WriteLine($"{x}) Exit");
                    }
                    else
                    {
                        Console.WriteLine($"{x}) {nerModelTypes[x]}");
                    }

                }
                Console.Write($"Please select a number 0 - {nerModelTypes.Length}: ");
                userInput = Console.ReadLine();
                if (Int32.TryParse(userInput, out parsedResult) && parsedResult != nerModelTypes.Length)
                {
                    results.Add(nerModelTypes[parsedResult]);
                }
                else if (parsedResult == nerModelTypes.Length)
                {
                    Console.WriteLine("Thank you I will add this to my list.");
                }
                else
                {
                    Console.WriteLine("That is not a valid option please try again.");
                }
            }
            // build new object to insert into database
            QaSettingsModel newDictonarySetting = new QaSettingsModel();
            newDictonarySetting.QuestionIdentifier = parseQuestionObj.QuestionIdentifier;
            newDictonarySetting.NerTypes = results;
            // add object into database
            _qaSettingsMethods.AddSingleEntry(newDictonarySetting);
        }
    }
}
