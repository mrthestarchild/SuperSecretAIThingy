using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionAnswerAi.Utils
{
    public class OpenNLPUtils
    {
        public Dictionary<string, string> TreeBankTags { get; set; }
        public Dictionary<string, string> ChunkerTags { get; set; }
        public Dictionary<string, string[]> QuestionDetermination { get; set; }
        public string[] NoTimeFrameDefs { get; set; }

        public OpenNLPUtils()
        {
            // Create TreeBankTags Dictionary to test passed in chunker definitions
            TreeBankTags = new Dictionary<string, string>
            {
                { "CC", "Coordinating conjunction" },
                { "CD", "Cardinal number" },
                { "DT", "Determiner" },
                { "EX", "Existential there" },
                { "FW", "Foreign word" },
                { "IN", "Preposition or subordinating conjunction" },
                { "JJ", "Adjective" },
                { "JJR", "Adjective, comparative" },
                { "JJS", "Adjective, superlative" },
                { "LS", "List item marker" },
                { "MD", "Modal" },
                { "NN", "Noun, singular or mass" },
                { "NNS", "Noun, plural" },
                { "NNP", "Proper noun, singular" },
                { "NNPS", "Proper noun, plural" },
                { "PDT", "Predeterminer" },
                { "POS", "Possessive ending" },
                { "PRP", "Personal pronoun" },
                { "PRP$", "Possessive pronoun" },
                { "RB", "Adverb" },
                { "RBR", "Adverb, comparative" },
                { "RBS", "Adverb, superlative" },
                { "RP", "Particle" },
                { "SYM", "Symbol" },
                { "TO", "to" },
                { "UH", "Interjection" },
                { "VB", "Verb, base form" },
                { "VBD", "Verb, past tense" },
                { "VBG", "Verb, gerund or present participle" },
                { "VBN", "Verb, past participle" },
                { "VBP", "Verb, non­3rd person singular present" },
                { "VBZ", "Verb, 3rd person singular present" },
                { "WDT", "Wh­-determiner" },
                { "WP", "Wh­-pronoun" },
                { "WP$", "Possessive wh­pronoun" },
                { "WRB", "Wh­-adverb" }
            };
         
                                                                                                                                                        
            // create ChunkerTags definitions to check for tag definitions
            ChunkerTags = new Dictionary<string, string>
            {
                { "NP", "Noun Phrase" },
                { "VP", "Verb Phrase" },
                { "PP", "Preposition" },
                { "ADVP", "Adverb Phrase"},
                { ".", "End of sentence" }
            };

            // create no time frame defs to define and find default times for now
            NoTimeFrameDefs = new string[] 
            {
                DateTime.Now.Year.ToString().ToLower(),
                "current",
                "now",
                "currently",
                "at the present time",
                "presently",
                "nowadays",
                "for the time being",
                "today",
                "right now"
            };



        }
    }
}
