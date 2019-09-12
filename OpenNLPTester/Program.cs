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

            Seperator("Tokenizer");
            // Test of Tokenizer with internal string.
            string tokenPath = $@"{modelsPath}EnglishTok.nbin";
            string sentence = "- Sorry Mrs. Hudson, I'll skip the tea I'll be back in October 5th 2019 malignant tumor.";
            EnglishMaximumEntropyTokenizer tokenizer = new EnglishMaximumEntropyTokenizer(tokenPath);
            string[] tokens = tokenizer.Tokenize(sentence);
            foreach (string token in tokens)
            {
                Console.WriteLine(token);
            }

            Seperator("Sentence Splitter");
            // Test of Sentence Splitter
            // TODO: Fix is it not splitting the sentences
            var paragraph = "Mr. & Mrs. Smith is a 2005 American romantic comedy action film. The film stars Brad Pitt and Angelina Jolie as a bored upper-middle class married couple. They are surprised to learn that they are both assassins hired by competing agencies to kill each other.";
            string modelPath = $@"{modelsPath}EnglishTok.nbin";
            var sentenceDetector = new EnglishMaximumEntropySentenceDetector(modelPath);
            var sentences = sentenceDetector.SentenceDetect(paragraph);
            foreach (string theSentence in sentences)
            {
                Console.WriteLine(theSentence);
            }

            Seperator("posTagger");
            // Test of posTagger from tokenized string
            string posPath = $@"{modelsPath}EnglishPOS.nbin";
            string dictPath = $@"{modelsPath}Parser\tagdict";
            EnglishMaximumEntropyPosTagger posTagger = new EnglishMaximumEntropyPosTagger(posPath, dictPath);
            string[] pos = posTagger.Tag(tokens);
            foreach (string p in pos)
            {
                Console.WriteLine(p);
            }

            Seperator("Named Entity Recognition");
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

            Seperator("Parse Tree");
            // TODO: Fix Parse and run secondary test. This is not working as expected.
            // test of the Parser
            EnglishTreebankParser parser = new EnglishTreebankParser(modelsPath);
            Parse parse = parser.DoParse(tokens);
            Console.WriteLine(parse);

            Seperator("Trainer");
            //test the trainer for ner
            // The file with the training samples; works also with an array of files
            string trainingFile = @"C:\Users\nerab\source\repos\OpenNLPTester\OpenNLPTester\Resources\Training\NER\disease.train";
            // The number of iterations; no general rule for finding the best value, just try several!
            int iterations = 100;
            // The cut; no general rule for finding the best value, just try several!
            int cut = 5;
            // TODO: this is not working as expected
            // Train the model (can take some time depending on your training file size)

            GisModel bestModel = MaximumEntropyNameFinder.TrainModel(trainingFile, iterations, cut);
            // Persist the model to use it later
            var outputFilePath = $@"{modelsPath}NameFind\disease.nbin";
            new BinaryGisModelWriter().Persist(bestModel, outputFilePath);

            Console.WriteLine("Done!");
            Console.ReadLine();

        }

        static void Seperator(string desc)
        {
            Console.WriteLine($"\n{desc}-----------------------------------------\n");
        }
    }
}
