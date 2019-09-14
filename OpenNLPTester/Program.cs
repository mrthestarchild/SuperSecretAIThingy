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
using OpenNLP.Tools.Lang.English;
using OpenNLP.Tools.Chunker;

namespace OpenNLPTester
{
    class Program
    {
        static void Main(string[] args)
        {
            string modelsPath = Directory.GetCurrentDirectory() + @"\Resources\Models\";
            // TODO: Create buffered reader for someone to enter question using args.
            string sentence = "- Sorry Mrs. Hudson, I'll skip the tea I'll be back in October 5th 2019 malignant tumor cancer.";
            string paragraph = "Mr. & Mrs. Smith is a 2005 American romantic comedy action film. The film stars Brad Pitt and Angelina Jolie as a bored upper-middle class married couple. They are surprised to learn that they are both assassins hired by competing agencies to kill each other.";

            OpenNLPMethods methods = new OpenNLPMethods();

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
            foreach (string p in pos)
            {
                Console.WriteLine(p);
            }

            Seperator("Named Entity Recognition");
            // Test of the Name entity recognition on currently trained models
            string ner = methods.NER(sentence);
            Console.WriteLine(ner);

            Seperator("Parse Tree");
            // test of the Parser
            string ptOutput = methods.ParseTree(sentence);
            Console.WriteLine(ptOutput);

            Seperator("Chunker");
            // test of the Chunker
            var chunks = methods.Chunker(sentence);
            foreach (var chunk in chunks)
            {
                Console.WriteLine(chunk);
            }

            Seperator("Coference");
            // TODO: Fix this thang
            //var coferenceModel = $@"{modelsPath}Coref\";
            //var coreferenceFinder = new TreebankLinker(coferenceModel);
            //string[] sentences = {"Mr. & Mrs. Smith is a 2005 American romantic comedy action film.",
            //    "The film stars Brad Pitt and Angelina Jolie as a bored upper-middle class married couple.",
            //    "They are surprised to learn that they are both assassins hired by competing agencies to kill each other." };
            //string coref = coreferenceFinder.GetCoreferenceParse(sentences);

            // TODO: Break out trainer into seperate training methods. This works for now to do static training until it has been fixed.
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
