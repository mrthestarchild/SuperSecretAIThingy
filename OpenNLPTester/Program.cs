using CommonServiceLocator;
using QuestionAnswerAi.Controllers;
using QuestionAnswerAi.Solr;
using QuestionAnswerAi.Solr.Models;
using QuestionAnswerAi.Utils;
using SolrNet;
using System;
using System.IO;
using System.Linq;
using QuestionAnswerAi.Models;

namespace QuestionAnswerAi
{
    class Program
    {
        static void Main(string[] args)
        {
            // do init of the SOLR collections set at this endpoint.

            Startup.Init<QaSettingsModel>("http://localhost:8983/solr/qasettings");
            Startup.Init<WikiModelResult>("http://localhost:8983/solr/wikitest");
            
            // get a sentence from the end user that we are going to check in SOLR collection
            Console.WriteLine("What would you like to know?: ");
            var sentence = Console.ReadLine();

            // add a question mark at the end if not there, this is needed for correct NLP processes
            sentence = sentence.Contains("?") ? sentence : $"{sentence}?";

            // get the path to the model's for OpenNLP
            string modelsPath = Directory.GetCurrentDirectory() + @"\Resources\Models\";

            // create an instance of the QuestionAnswerController, methods used in web api.
            var external = new QuestionAnswerController(modelsPath);

            // ask the question and get response.
            var foundAnswer = external.GetAnswerFromQuestion(sentence);

            // write out answer to console.
            Seperator("Answers");
            Console.WriteLine(foundAnswer != null
                ? $"{foundAnswer.Answer} score = {foundAnswer.Score}"
                : "I couldn't find any answers to that question.");

            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        static void Seperator(string desc)
        {
            Console.WriteLine($"\n{desc}-----------------------------------------\n");
        }
    }
}
