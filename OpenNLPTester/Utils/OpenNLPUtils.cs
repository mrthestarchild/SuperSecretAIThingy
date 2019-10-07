using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLPTester.Utils
{
    class OpenNLPUtils
    {
        public IDictionary<string, string> posDefs = new Dictionary<string, string>();

        OpenNLPUtils()
        {
            posDefs.Add("CC", "Coordinating conjunction");
            posDefs.Add("CD", "Cardinal number");
            posDefs.Add("DT", "Determiner");
            posDefs.Add("EX", "Existential there");
            posDefs.Add("FW", "Foreign word");
            posDefs.Add("IN", "Preposition or subordinating conjunction");
            posDefs.Add("JJ", "Adjective");
            posDefs.Add("JJR", "Adjective, comparative");
            posDefs.Add("JJS", "Adjective, superlative");
            posDefs.Add("LS", "List item marker");
            posDefs.Add("MD", "Modal");
            posDefs.Add("NN", "Noun, singular or mass");
            posDefs.Add("NNS", "Noun, plural");
            posDefs.Add("NNP", "Proper noun, singular");
            posDefs.Add("NNPS", "Proper noun, plural");
            posDefs.Add("PDT", "Predeterminer");
            posDefs.Add("POS", "Possessive ending");
            posDefs.Add("PRP", "Personal pronoun");
            posDefs.Add("PRP$", "Possessive pronoun");
            posDefs.Add("RB", "Adverb");
            posDefs.Add("RBR", "Adverb, comparative");
            posDefs.Add("RBS", "Adverb, superlative");
            posDefs.Add("RP", "Particle");
            posDefs.Add("SYM", "Symbol");
            posDefs.Add("TO", "to");
            posDefs.Add("UH", "Interjection");
            posDefs.Add("VB", "Verb, base form");
            posDefs.Add("VBD", "Verb, past tense");
            posDefs.Add("VBG", "Verb, gerund or present participle");
            posDefs.Add("VBN", "Verb, past participle");
            posDefs.Add("VBP", "Verb, non­3rd person singular present");
            posDefs.Add("VBZ", "Verb, 3rd person singular present");
            posDefs.Add("WDT", "Wh­determiner");
            posDefs.Add("WP", "Wh­pronoun");
            posDefs.Add("WP$", "Possessive wh­pronoun");
            posDefs.Add("WRB", "Wh­adverb");
        }
    }
}
