using System;
using System.Collections.Generic;
using System.IO;
using OpenNLP.Tools.NameFind;
using SharpEntropy.IO;
using SharpEntropy;
using SolrNet;
using SolrNet.Attributes;
using CommonServiceLocator;
using System.Linq;
using System.Text.RegularExpressions;
using OpenNLP.Tools.Lang.English;
using OpenNLP.Tools.Coreference;

namespace OpenNLPTester
{
    class Program
    {
        //class Product
        //{
        //    [SolrUniqueKey("id")]
        //    public string Id { get; set; }

        //    [SolrField("manu")]
        //    public string Manufacturer { get; set; }

        //    [SolrField("cat")]
        //    public ICollection<string> Categories { get; set; }

        //    [SolrField("price")]
        //    public decimal Price { get; set; }

        //    [SolrField("inStock")]
        //    public bool InStock { get; set; }
        //}
        static void Main(string[] args)
        {
            // TODO: break out into own method. Maybe use dependency injection for startup
            // Starting testing of solr in main

            // TODO: need to spin up server to be able to test this 
            //Startup.Init<Product>("http://localhost:8983/solr");

            //var p = new Product
            //{
            //    Id = "SP2514M",
            //    Manufacturer = "Samsung Electronics Co. Ltd.",
            //    Categories = new[] {
            //    "electronics",
            //    "hard drive",
            //},
            //    Price = 92,
            //    InStock = true,
            //};

            //var solr = ServiceLocator.Current.GetInstance<ISolrConnection>();

            //Dictionary<Product, double?> dict = new Dictionary<Product, double?>();
            //dict.Add(p, 0);

            //Console.WriteLine("-- Add products --");
            //AddProducts(solr, dict);

            //Console.WriteLine("-- Commit changes --");
            //CommitChanges(solr);

            //Console.WriteLine("--Query all documents --");
            //QueryAll();

            //Console.WriteLine("-- Delete all documents --");
            //DeleteAll(solr);

            //Console.WriteLine("-- Commit changes --");
            //CommitChanges(solr);

            //Console.WriteLine("--Query all documents --");
            //QueryAll();

            //Console.ReadKey();

            // start of openNLP usage
            string modelsPath = Directory.GetCurrentDirectory() + @"\Resources\Models\";
            string trainingPath = Directory.GetCurrentDirectory() + @"\Resources\Training\";
            // TODO: Create buffered reader for someone to enter question using args.
            string sentence = "- Sorry Mrs. Hudson, I'll skip the tea I'll be back in October 5th 2019. I am headed from Spain to Munich then I am going to stop in Heidelberg.";
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
            var coferenceModel = $@"{modelsPath}Coref\";
            var coreferenceFinder = new TreebankLinker(coferenceModel, LinkerMode.Test);
            string[] sentences = {"Mr. & Mrs. Smith is a 2005 American romantic comedy action film.",
                "The film stars Brad Pitt and Angelina Jolie as a bored upper-middle class married couple.",
                "They are surprised to learn that they are both assassins hired by competing agencies to kill each other." };
            string coref = coreferenceFinder.GetCoreferenceParse(sentences);

            // TODO: Break out trainer into seperate training methods. This works for now to do static training until it has been fixed.
            Seperator("Name Finder Trainer");
            // Test the trainer for ner
            // The file with the training samples; works also with an array of files
            string trainingFile = $@"{trainingPath}NER\combined.train";
            // The number of iterations; no general rule for finding the best value, just try several!
            int[] iterations = { 100, 200, 1000, 5000 };
            // The cut; no general rule for finding the best value, just try several!
            int[] cuts = { 1, 2, 3, 4, 5, 10, 20 };

            // init values
            var bestIterationValue = iterations[0];
            var bestCutValue = cuts[0];
            var initNumFinds = 0;
            var bestAccuracy = 0;
            // get a base model to init the best model, if no change then we will need to try again again.
            GisModel bestModel = MaximumEntropyNameFinder.TrainModel(trainingFile, bestIterationValue, bestCutValue);
            
            // TODO: this is not working as expected
            // Train the model (can take some time depending on your training file size)
            foreach (int iteration in iterations)
            {
                foreach (int cut in cuts)
                {
                    // train model
                    GisModel model = MaximumEntropyNameFinder.TrainModel(trainingFile, iteration, cut);
                    // test model
                    string modelTestResult = methods.NER(sentence);

                    // set number of finds for what we are matching to
                    var numOfFinds = Regex.Matches(modelTestResult, "place").Count;
                    
                    // check if we find anything, if we do we set the values to know what the best system was and set the new model to bestModel.
                    if (numOfFinds >= initNumFinds)
                    {
                        bestAccuracy = numOfFinds;
                        bestIterationValue = iteration;
                        bestCutValue = cut;
                        bestModel = model;
                    }
                }
            }

            // Write out our findings.
            Console.WriteLine($"The best accuracy was {bestAccuracy}, the best iteration value was {bestIterationValue}, and the best cutoff value was {bestCutValue}.");

            // Persist the model to use it later
            var outputFilePath = $@"{modelsPath}NameFind\place.nbin";
            new BinaryGisModelWriter().Persist(bestModel, outputFilePath); ;

            Console.WriteLine("Done!");
            Console.ReadLine();

        }

        static void Seperator(string desc)
        {
            Console.WriteLine($"\n{desc}-----------------------------------------\n");
        }

        //private static void DeleteAll(ISolrConnection solr)
        //{

        //    DeleteParameters del = new DeleteParameters();
        //    SolrNet.Impl.FieldSerializers.DefaultFieldSerializer deffield = new SolrNet.Impl.FieldSerializers.DefaultFieldSerializer();
        //    SolrNet.Impl.QuerySerializers.DefaultQuerySerializer defquery = new SolrNet.Impl.QuerySerializers.DefaultQuerySerializer(deffield);

        //    SolrNet.Commands.Parameters.DeleteByIdAndOrQueryParam delpar = new SolrNet.Commands.Parameters.DeleteByIdAndOrQueryParam(Enumerable.Empty<string>(),SolrQuery.All, defquery);
        //    var delete = new SolrNet.Commands.DeleteCommand(delpar, del);
        //    string res = delete.Execute(solr);
        //    System.Diagnostics.Trace.WriteLine(res);
        //}

        //private static void CommitChanges(ISolrConnection solr)
        //{
        //    SolrNet.Commands.CommitCommand commit = new SolrNet.Commands.CommitCommand();
        //    string res = commit.Execute(solr);
        //    System.Diagnostics.Trace.WriteLine(res);
        //}

        //private static void AddProducts(ISolrConnection solr, Dictionary<Product, double?> dict)
        //{
        //    AddParameters par = new AddParameters();
        //    ISolrDocumentSerializer<Product> ser = ServiceLocator.Current.GetInstance<ISolrDocumentSerializer<Product>>();
        //    SolrNet.Commands.AddCommand<Product> addProduct = new SolrNet.Commands.AddCommand<Product>(dict, ser, par);
        //    string res = addProduct.Execute(solr);
        //    System.Diagnostics.Trace.WriteLine(res);
        //}

        //private static void QueryAll()
        //{
        //    // Query all documents
        //    var query = SolrQuery.All;
        //    var operations = ServiceLocator.Current.GetInstance<ISolrOperations<Product>>();
        //    var Products = operations.Query(query);

        //    int i = 0;
        //    foreach (var product in Products)
        //    {
        //        i++;
        //        Console.WriteLine("{0}:\t {1} \t{2} \t{3}", i, product.Id, product.Manufacturer, product.Price);
        //    }

        //    if (i == 0)
        //    {
        //        Console.WriteLine(" = no documents =");
        //    }
        //}
    }
}
