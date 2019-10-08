using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLPTester.Utils
{
    public class OpenNLPUtils
    {
        public Dictionary<string, string> TreeBankTags { get; set; }
        public Dictionary<string, string> ChunkerTags { get; set; }

        public OpenNLPUtils()
        {
            // Create TreeBankTags Dictionary to test passed in chunker definitions
            TreeBankTags = new Dictionary<string, string>();
            TreeBankTags.Add("CC", "Coordinating conjunction");
            TreeBankTags.Add("CD", "Cardinal number");
            TreeBankTags.Add("DT", "Determiner");
            TreeBankTags.Add("EX", "Existential there");
            TreeBankTags.Add("FW", "Foreign word");
            TreeBankTags.Add("IN", "Preposition or subordinating conjunction");
            TreeBankTags.Add("JJ", "Adjective");
            TreeBankTags.Add("JJR", "Adjective, comparative");
            TreeBankTags.Add("JJS", "Adjective, superlative");
            TreeBankTags.Add("LS", "List item marker");
            TreeBankTags.Add("MD", "Modal");
            TreeBankTags.Add("NN", "Noun, singular or mass");
            TreeBankTags.Add("NNS", "Noun, plural");
            TreeBankTags.Add("NNP", "Proper noun, singular");
            TreeBankTags.Add("NNPS", "Proper noun, plural");
            TreeBankTags.Add("PDT", "Predeterminer");
            TreeBankTags.Add("POS", "Possessive ending");
            TreeBankTags.Add("PRP", "Personal pronoun");
            TreeBankTags.Add("PRP$", "Possessive pronoun");
            TreeBankTags.Add("RB", "Adverb");
            TreeBankTags.Add("RBR", "Adverb, comparative");
            TreeBankTags.Add("RBS", "Adverb, superlative");
            TreeBankTags.Add("RP", "Particle");
            TreeBankTags.Add("SYM", "Symbol");
            TreeBankTags.Add("TO", "to");
            TreeBankTags.Add("UH", "Interjection");
            TreeBankTags.Add("VB", "Verb, base form");
            TreeBankTags.Add("VBD", "Verb, past tense");
            TreeBankTags.Add("VBG", "Verb, gerund or present participle");
            TreeBankTags.Add("VBN", "Verb, past participle");
            TreeBankTags.Add("VBP", "Verb, non­3rd person singular present");
            TreeBankTags.Add("VBZ", "Verb, 3rd person singular present");
            TreeBankTags.Add("WDT", "Wh­-determiner");
            TreeBankTags.Add("WP", "Wh­-pronoun");
            TreeBankTags.Add("WP$", "Possessive wh­pronoun");
            TreeBankTags.Add("WRB", "Wh­-adverb");

            // create ChunkerTags definitions to check for tag definitions
            ChunkerTags = new Dictionary<string, string>();
            ChunkerTags.Add("NP", "Noun Phrase");
            ChunkerTags.Add("VP", "Verb Phrase");
            ChunkerTags.Add("PP", "Preposition");
            ChunkerTags.Add(".", "End of sentence");
        }
    }
}
