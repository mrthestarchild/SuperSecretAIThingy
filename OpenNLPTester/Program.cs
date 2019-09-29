using System;
using System.Collections.Generic;
using System.IO;
using SharpEntropy.IO;
using SharpEntropy;
using SolrNet;
using SolrNet.Attributes;
using CommonServiceLocator;
using System.Linq;
using OpenNLP.Tools.Coreference;
using OpenNLP.Tools.Lang.English;
using OpenNLPTester.Solr.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;

namespace OpenNLPTester
{
    class Program
    {
        static void Main(string[] args)
        {
            // Starting testing of solr in main

            //// Temp SolrWebRequest
            //WebRequest request = WebRequest.Create("http://localhost:8983/solr/techproducts/select?q=test");
            //WebResponse response = request.GetResponse();
            //// Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            //using (Stream dataStream = response.GetResponseStream())
            //{
            //    // Open the stream using a StreamReader for easy access.  
            //    StreamReader reader = new StreamReader(dataStream);
            //    // Read the content.  
            //    string responseFromServer = reader.ReadToEnd();
            //    // Display the content.
            //    dynamic array = JsonConvert.DeserializeObject(responseFromServer);
            //    foreach (var item in array.response)
            //    {
            //        Console.WriteLine(item);
            //    }
            //    Console.WriteLine("------------------------------------------------------------------------------------------");

            //}
            //// Close the response.  
            //response.Close();


            // Usage of Solr Query using Solr.net
            Startup.Init<techproducts>("http://localhost:8983/solr/techproducts");
            var solr = ServiceLocator.Current.GetInstance<ISolrOperations<techproducts>>();
            var results = solr.Query("div");
            Console.WriteLine($"Count = {results.Count}");
            foreach (var result in results)
            {
                Console.WriteLine($"content = {result.Content.First<string>()}");
            }

            // start of openNLP usage

            // get the paths for the Model and Training files
            string modelsPath = Directory.GetCurrentDirectory() + @"\Resources\Models\";
            string trainingPath = Directory.GetCurrentDirectory() + @"\Resources\Training\";
            // TODO: Create buffered reader for someone to enter question using args.
            string defaultSentence = "- Sorry Mrs. Hudson, I'll skip the tea I'll be back in October 5th 2019. I am headed from Spain to Munich then I am going to stop in Heidelberg.";
            string defaultParagraph = "Mr. & Mrs. Smith is a 2005 American romantic comedy action film. The film stars Brad Pitt and Angelina Jolie as a bored upper-middle class married couple. They are surprised to learn that they are both assassins hired by competing agencies to kill each other.";

            Console.WriteLine("Please enter a sentence to parse: ");
            var sentence = Console.ReadLine();
            sentence = sentence == "" ? defaultSentence : sentence;

            Console.WriteLine("Please enter a paragraph to parse: ");
            var paragraph = Console.ReadLine();
            paragraph = paragraph == "" ? defaultParagraph : paragraph; 

            // Create an access point for methods and trainers.
            OpenNLPMethods methods = new OpenNLPMethods(modelsPath);
            OpenNLPTrainers trainer = new OpenNLPTrainers(methods, trainingPath);

            Seperator("Tokenizer");
            string[] tokens = methods.Tokenize(sentence);
            foreach (string token in tokens)
            {
                Console.WriteLine(token);
            }

            Seperator("Sentence Splitter");
            // Test of Sentence Splitter
            string[] sp = methods.SentenceSplitter(paragraph);
            foreach (string spSent in sp)
            {
                Console.WriteLine(spSent);
            }

            Seperator("posTagger");
            // Test of posTagger from tokenized string
            string[] pos = methods.POSTagger(sentence);
            foreach (string po in pos)
            {
                Console.WriteLine(po);
            }

            Seperator("Named Entity Recognition");
            // Test of the Name entity recognition on currently trained models
            string ner = methods.NER(sentence);
            Console.WriteLine(ner);

            Seperator("Parse Tree");
            // Test of the Parser
            string ptOutput = methods.ParseTree(sentence);
            Console.WriteLine(ptOutput);

            Seperator("Chunker");
            // Test of the Chunker
            var chunks = methods.Chunker(sentence);
            foreach (var chunk in chunks)
            {
                Console.WriteLine(chunk);
            }

            Seperator("Coference");
            // TODO: Fix this thang
            //var coferenceModel = $@"{modelsPath}Coref\";

            //var conreferenceFinder = new TreebankLinker(coferenceModel);
            
            //string[] sentences = {"Mr. & Mrs. Smith is a 2005 American romantic comedy action film.",
            //    "The film stars Brad Pitt and Angelina Jolie as a bored upper-middle class married couple.",
            //    "They are surprised to learn that they are both assassins hired by competing agencies to kill each other." };
            //var coref = conreferenceFinder.GetCoreferenceParse(sentences);

            Seperator("Name Finder Trainer");
            // get variables ready for training
            // how many times we are going to loop over the file to train
            int[] iterations = {10, 20, 50, 100, 500, 1000, 5000 };
            // how many unique entries we are going to look for
            int[] cuts = { 1, 2, 5, 10, 20, 50, 100 };
            // what types we are tring to train for NER
            string[] listOfTypes = { "places" };

            // train model, this will output if it was succesful or not.
            //var bestNERModel = trainer.TrainNER(sentence, iterations, cuts, "combined.train", listOfTypes);

            //persist model and save it for later.
            //var outputFilePath = $@"{modelsPath}NameFind\places.nbin";
            //new BinaryGisModelWriter().Persist(bestNERModel, outputFilePath);
            Console.WriteLine("Done!");
            Console.ReadKey();
        }

        static void Seperator(string desc)
        {
            Console.WriteLine($"\n{desc}-----------------------------------------\n");
        }
    }
}
