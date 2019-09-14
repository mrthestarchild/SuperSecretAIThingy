using OpenNLP.Tools.Chunker;
using OpenNLP.Tools.NameFind;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.SentenceDetect;
using OpenNLP.Tools.Tokenize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLPTester
{
    class OpenNLPMethods
    {
        // TODO: Create overloads for methods
        string modelsPath = Directory.GetCurrentDirectory() + @"\Resources\Models\";

        public string[] Tokenize(string sentence, string tokenPath = null)
        {
            if (tokenPath == null)
            {
                tokenPath = $@"{modelsPath}EnglishTok.nbin";
            }

            EnglishMaximumEntropyTokenizer tokenizer = new EnglishMaximumEntropyTokenizer(tokenPath);
            return tokenizer.Tokenize(sentence);
        }

        public string[] SentenceSplitter(string paragraph, string spPath = null)
        {
            if (spPath == null)
            {
                spPath = $@"{modelsPath}EnglishSD.nbin";
            }
            EnglishMaximumEntropySentenceDetector sentenceDetector = new EnglishMaximumEntropySentenceDetector(spPath);
            return sentenceDetector.SentenceDetect(paragraph);
        }

        public string[] POSTagger(string sentence, string posBinPath = null, string tagDictPath = null)
        {
            if (posBinPath == null)
            {
                posBinPath = $@"{modelsPath}EnglishPOS.nbin";
            }
            if (tagDictPath == null)
            {
                tagDictPath = $@"{modelsPath}Parser\tagdict";
            }
            string[] sentenceArray = Tokenize(sentence);
            EnglishMaximumEntropyPosTagger posTagger = new EnglishMaximumEntropyPosTagger(posBinPath, tagDictPath);
            return posTagger.Tag(sentenceArray);

        }

        public string ParseTree(string sentence, string modelPath = null)
        {
            if (modelPath == null)
            {
                modelPath = modelsPath;
            }
            EnglishTreebankParser parser = new EnglishTreebankParser(modelPath, true, false);
            Parse parse = parser.DoParse(sentence);
            return parse.Show();
        }

        public List<SentenceChunk> Chunker(string sentence, string chunkerModelPath = null)
        {
            if (chunkerModelPath == null)
            {
                chunkerModelPath = $@"{modelsPath}EnglishChunk.nbin";
            }
            EnglishTreebankChunker chunker = new EnglishTreebankChunker(chunkerModelPath);
            string[] chunkerTokens = Tokenize(sentence);
            string[] chunkerPOS = POSTagger(sentence);
            return chunker.GetChunks(chunkerTokens, chunkerPOS);
        }

        public string NER(string sentence, string nameFindPath = null, string[] nerModels = null)
        {
            if (nameFindPath == null)
            {
                nameFindPath = $@"{modelsPath}NameFind\";
            }
            if (nerModels == null)
            {
                string[] files = Directory.GetFiles(nameFindPath);
                int filesLength = files.Length;
                string[] currentModels = new string[filesLength];
                //create list of model types from models folder
                for (int x = 0; x < filesLength; x++)
                {
                    currentModels[x] = Path.GetFileName(files[x]);
                    // sanitize file name
                    currentModels[x] = currentModels[x].Replace(".nbin", "");
                }
                nerModels = currentModels;
            }
            EnglishNameFinder nameFinder = new EnglishNameFinder(nameFindPath);
            // specify which types of entities you want to detect
            return nameFinder.GetNames(nerModels, sentence);
        }

        // TODO: create chunker method

        // TODO: create coreference method.

    }
}
