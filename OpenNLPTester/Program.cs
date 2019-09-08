using OpenNLP.Tools.Tokenize;
using OpenNLP.Tools.PosTagger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNLP.Tools.NameFind;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.SentenceDetect;
using SharpEntropy.IO;
using SharpEntropy;

namespace OpenNLPTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string modelsPath = @"C:\Users\nerab\source\repos\OpenNLPTester\OpenNLPTester\Resources\Models\";

            // Test of Tokenizer with internal string.
            string tokenPath = $@"{modelsPath}EnglishTok.nbin";
            string sentence = "- Sorry Mrs. Hudson, I'll skip the tea I'll be back in October of 2019.";
            EnglishMaximumEntropyTokenizer tokenizer = new EnglishMaximumEntropyTokenizer(tokenPath);
            string[] tokens = tokenizer.Tokenize(sentence);
            foreach (string token in tokens)
            {
                Console.WriteLine(token);
            }

            // Test of posTagger from tokenized string
            string posPath = $@"{modelsPath}EnglishPOS.nbin";
            string dictPath = $@"{modelsPath}Parser\tagdict";
            EnglishMaximumEntropyPosTagger posTagger = new EnglishMaximumEntropyPosTagger(posPath, dictPath);
            string[] pos = posTagger.Tag(tokens);
            foreach (string p in pos)
            {
                Console.WriteLine(p);
            }

            // Test of the Name entity recognition on currently trained models
            string nerPath = $@"{modelsPath}NameFind\";
            EnglishNameFinder nameFinder = new EnglishNameFinder(nerPath);
            // specify which types of entities you want to detect
            string[] files = Directory.GetFiles(nerPath);
            int filesLength = files.Length;
            string[] currentModels = new string[filesLength];
            //create list of model types from models folder
            for (int x = 0; x < filesLength; x++)
            {
                currentModels[x] = Path.GetFileName(files[x]);
                // sanitize file name
                currentModels[x] = currentModels[x].Replace(".nbin", "");
            }
            string ner = nameFinder.GetNames(currentModels, sentence);
            Console.WriteLine(ner);

            // TODO: Fix Parse and run secondary test. This is not working as expected.
            // test of the Parser
            EnglishTreebankParser parser = new EnglishTreebankParser(modelsPath);
            Parse parse = parser.DoParse(sentence);
            Console.WriteLine(parse);

            //test the trainer for ner
            // The file with the training samples; works also with an array of files
            string trainingFile = @"C:\Users\nerab\source\repos\OpenNLPTester\OpenNLPTester\Resources\Training\NER\disease.train";
            // The number of iterations; no general rule for finding the best value, just try several!
            int iterations = 5;
            // The cut; no general rule for finding the best value, just try several!
            int cut = 2;
            // TODO: this is broken right now with a null pointer exception. can't figure out why?
            // Train the model (can take some time depending on your training file size)
            GisModel bestModel;
            try
            {
                bestModel = MaximumEntropyNameFinder.TrainModel(trainingFile, iterations, cut);
                // Persist the model to use it later
                var outputFilePath = $@"{modelsPath}NameFind\";
                new BinaryGisModelWriter().Persist(bestModel, outputFilePath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"dumb: {e}");
            }
            

            Console.ReadLine();

        }
    }
}
