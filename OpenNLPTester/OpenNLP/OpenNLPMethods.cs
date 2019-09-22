using OpenNLP.Tools.Chunker;
using OpenNLP.Tools.NameFind;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.PosTagger;
using OpenNLP.Tools.SentenceDetect;
using OpenNLP.Tools.Tokenize;
using System.Collections.Generic;
using System.IO;

namespace OpenNLPTester
{
    class OpenNLPMethods
    {
        private string modelsPath;

        public OpenNLPMethods(string _modelsPath){
            modelsPath = _modelsPath;
        }

        /// <summary>
        /// Tokenizes a sentence.
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="tokenPath"></param>
        /// <returns>Array of words</returns>
        public string[] Tokenize(string sentence, string tokenPath = null)
        {
            if (tokenPath == null)
            {
                tokenPath = $@"{modelsPath}EnglishTok.nbin";
            }

            EnglishMaximumEntropyTokenizer tokenizer = new EnglishMaximumEntropyTokenizer(tokenPath);
            return tokenizer.Tokenize(sentence);
        }

        /// <summary>
        /// Splits paragraphs into their own sentences
        /// </summary>
        /// <param name="paragraph"></param>
        /// <param name="spPath"></param>
        /// <returns>Array of sentences</returns>
        public string[] SentenceSplitter(string paragraph, string spPath = null)
        {
            if (spPath == null)
            {
                spPath = $@"{modelsPath}EnglishSD.nbin";
            }
            EnglishMaximumEntropySentenceDetector sentenceDetector = new EnglishMaximumEntropySentenceDetector(spPath);
            return sentenceDetector.SentenceDetect(paragraph);
        }

        /// <summary>
        /// Tokenizes then tags sentence with their types of words.
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="posBinPath"></param>
        /// <param name="tagDictPath"></param>
        /// <returns>Only the types ie.(noun, verb, adverb)</returns>
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

        /// <summary>
        /// Tags a array of questions and their types for a simple inference.
        /// </summary>
        /// <param name="questions"></param>
        /// <param name="posBinPath"></param>
        /// <param name="tagDictPath"></param>
        /// <returns></returns>
        // TODO: fix this guy
        public string[] POSQuestionTagger(string[] questions, string posBinPath = null, string tagDictPath = null)
        {
            if (posBinPath == null)
            {
                // TODO: Train this nbin file
                posBinPath = $@"{modelsPath}EnglishQuestionPOS.nbin";
            }
            if (tagDictPath == null)
            {
                tagDictPath = $@"{modelsPath}Parser\tagdict";
            }
            EnglishMaximumEntropyPosTagger posTagger = new EnglishMaximumEntropyPosTagger(posBinPath, tagDictPath);
            return posTagger.Tag(questions);

        }

        /// <summary>
        /// Parses with it's POS tags and returns parsed string in full context 
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="modelPath"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Tokenizes, POS tags, and returns full value
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="chunkerModelPath"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Looks up model types in model directory then checks for types in sentence, returns Named Entity Recognition tags on sentece.
        /// </summary>
        /// <param name="sentence"></param>
        /// <param name="nameFindPath"></param>
        /// <param name="nerModels"></param>
        /// <returns></returns>
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

        // TODO: create coreference method.

    }
}
