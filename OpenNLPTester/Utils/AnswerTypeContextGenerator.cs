// TODO: Remove, depricated.
using java.io;
using java.util.regex;
using OpenNLP.Tools.Coreference.Mention;
using OpenNLP.Tools.Parser;
using OpenNLP.Tools.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestionAnswerAi.Utils
{
    class AnswerTypeContextGenerator
    {
        private static Pattern falseHeadsPattern = Pattern.compile("^(name|type|kind|sort|form|one|breed|names|variety)$");
        private static Pattern copulaPattern = Pattern.compile("^(is|are|'s|were|was|will)$");
        private static Pattern queryWordPattern = Pattern.compile("^(who|what|when|where|why|how|whom|which|name)$");
        private static Pattern useFocusNounPattern = Pattern.compile("^(who|what|which|name)$");
        private static Pattern howModifierTagPattern = Pattern.compile("^(JJ|RB)");
        private WordnetDictionary wordNet;

        public AnswerTypeContextGenerator(File dictDir)
        {
            try
            {
                wordNet = new WordnetDictionary(dictDir.getAbsolutePath());
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Unable to initalize", e);
            }
        }

        private bool isQueryWord(string word)
        {
            return (queryWordPattern.matcher(word).matches());
        }

        private int movePastPrep(int i, Parse[] toks)
        {
            if (i < toks.Length && (toks[i].ToString().Equals("of") || toks[i].ToString().Equals("for")))
            {
                i++;
            }
            return (i);
        }

        private int movePastOf(int i, Parse[] toks)
        {
            if (i < toks.Length && toks[i].ToString().Equals("of"))
            {
                i++;
            }
            return (i);
        }

        private int movePastCopula(int i, Parse[] toks)
        {
            var toksCheck = toks[i].ToString();
            if (i < toks.Length && toksCheck.StartsWith("V"))
            {
                if (copulaPattern.matcher(toks[i].ToString()).matches())
                {
                    i++;
                }
            }
            return (i);
        }


        private Parse[] getNounPhrases(Parse parse)
        {
            List<Parse> nps = new List<Parse>(10);
            List<Parse> parts = new List<Parse>();
            parts.Add(parse);
            while (parts.Count > 0)
            {
                List<Parse> newParts = new List<Parse>();
                for (int pi = 0, pn = parts.Count; pi < pn; pi++)
                {
                    Parse cp = parts.ElementAt(pi);
                    //TODO: make sure IsLeaf works for isFlat as replacement.
                    if (cp.GetType().Equals("NP") && cp.IsLeaf)
                    {
                        nps.Add(cp);
                    }
                    else if (!cp.IsPosTag)
                    {
                        newParts.AddRange(cp.GetChildren());
                    }
                }
                parts = newParts;
            }
            return nps.ToArray();
        }

        private Parse getContainingNounPhrase(Parse token)
        {
            Parse parent = token.Parent;
            if (parent.GetType().Equals("NP"))
            {
                return parent;
            }
            return null;
        }

        private int getTokenIndexFollowingPhrase(Parse p, Parse[] toks)
        {
            Parse[] ptok = p.GetTagNodes();
            Parse lastToken = ptok[ptok.Length - 1];
            for (int ti = 0, tl = toks.Length; ti < tl; ti++)
            {
                if (toks[ti] == lastToken)
                {
                    return (ti + 1);
                }
            }
            return (toks.Length);
        }


        private Parse findFocusNounPhrase(String queryWord, int qwi, Parse[] toks)
        {
            if (queryWord.Equals("who"))
            {
                int npStart = movePastCopula(qwi + 1, toks);
                if (npStart > qwi + 1)
                { // check to ensure there is a copula
                    Parse np = getContainingNounPhrase(toks[npStart]);
                    if (np != null)
                    {
                        return (np);
                    }
                }
            }
            else if (queryWord.Equals("what"))
            {
                int npStart = movePastCopula(qwi + 1, toks);
                Parse np = getContainingNounPhrase(toks[npStart]);
                //removed copula case 
                if (np != null)
                {
                    Parse head = np.Head;
                    if (falseHeadsPattern.matcher(head.ToString()).matches())
                    {
                        npStart += np.GetChildren().Length;
                        int np2Start = movePastPrep(npStart, toks);
                        if (np2Start > npStart)
                        {
                            Parse snp = getContainingNounPhrase(toks[np2Start]);
                            if (snp != null)
                            {
                                return (snp);
                            }
                        }
                    }
                    return (np);
                }
            }
            else if (queryWord.Equals("which"))
            {
                //check for false query words like which VBD
                int npStart = movePastCopula(qwi + 1, toks);
                if (npStart > qwi + 1)
                {
                    return (getContainingNounPhrase(toks[npStart]));
                }
                else
                {
                    npStart = movePastOf(qwi + 1, toks);
                    return (getContainingNounPhrase(toks[npStart]));
                }
            }
            else if (queryWord.Equals("how"))
            {
                if (qwi + 1 < toks.Length)
                {
                    return (getContainingNounPhrase(toks[qwi + 1]));
                }
            }
            else if (qwi == 0 && queryWord.Equals("name"))
            {
                int npStart = qwi + 1;
                Parse np = getContainingNounPhrase(toks[npStart]);
                if (np != null)
                {
                    Parse head = np.Head;
                    if (falseHeadsPattern.matcher(head.ToString()).matches())
                    {
                        npStart += np.GetChildren().Length;
                        int np2Start = movePastPrep(npStart, toks);
                        if (np2Start > npStart)
                        {
                            Parse snp = getContainingNounPhrase(toks[np2Start]);
                            if (snp != null)
                            {
                                return (snp);
                            }
                        }
                    }
                    return (np);
                }
            }
            return (null);
        }

        private string[] getLemmas(Parse np)
        {
            // make sure we're getting a single word.
            string word = np.Head.ToString().ToLower();
            return wordNet.GetLemmas(word, "NN");
        }

        private Set<string> getSynsetSet(Parse np)
        {

            Set<string> synsetSet = new OpenNLP.Tools.Util.HashSet<string>();
            string[] lemmas = getLemmas(np);
            for (int li = 0; li < lemmas.Length; li++)
            {
                string[] synsets = wordNet.GetParentSenseKeys(lemmas[li], "NN", 0);
                for (int si = 0, sn = synsets.Length; si < sn; si++)
                {
                    synsetSet.Add(synsets[si]);
                }
            }
            return (synsetSet);
        }

        private void generateWordNetFeatures(Parse focusNoun, List<string> features)
        {

            Parse[] toks = focusNoun.GetTagNodes();
            string nnp = toks[toks.Length - 1].ToString();
            if (nnp.StartsWith("NNP"))
            {
                return;
            }
            //check wordnet 
            Set<string> synsets = getSynsetSet(focusNoun);

            foreach (string synset in synsets)
            {
                features.Add("s=" + synset);
            }
        }

        private void generateWordFeatures(Parse focusNoun, List<string> features)
        {
            Parse[] toks = focusNoun.GetTagNodes();
            int nsi = 0;
            for (; nsi < toks.Length - 1; nsi++)
            {
                features.Add("mw=" + toks[nsi]);
                features.Add("mt=" + toks[nsi].GetType());
            }
            features.Add("hw=" + toks[nsi]);
            features.Add("ht=" + toks[nsi].GetType());
        }

        public string[] getContext(Parse query)
        {
            Parse focalNoun = null;
            string queryWord = null;
            List<string> features = new List<string>();
            features.Add("def");
            Parse[] nps = getNounPhrases(query);
            Parse[] toks = query.GetTagNodes();
            int fnEnd = 0;
            int i = 0;
            bool fnIsLast = false;
            for (; i < toks.Length; i++)
            {
                string tok = toks[i].ToString().ToLower();
                if (isQueryWord(tok))
                {
                    queryWord = tok;
                    focalNoun = findFocusNounPhrase(queryWord, i, toks);
                    if (tok.Equals("how") && i + 1 < toks.Length)
                    {
                        if (howModifierTagPattern.matcher(toks[i + 1].GetType().ToString()).find())
                        {
                            queryWord = tok + "_" + toks[i + 1].ToString();
                        }
                    }
                    if (focalNoun != null)
                    {
                        fnEnd = getTokenIndexFollowingPhrase(focalNoun, toks);
                    }
                    if (focalNoun != null && focalNoun.Equals(nps[nps.Length - 1]))
                    {
                        fnIsLast = true;
                    }
                    break;
                }
            }
            int ri = i + 1;
            if (focalNoun != null)
            {
                ri = fnEnd + 1;
            }
            for (; ri < toks.Length; ri++)
            {
                features.Add("rw=" + toks[ri].ToString());
            }
            if (queryWord != null)
            {
                features.Add("qw=" + queryWord);
                string verb = null;
                //skip first verb for some query words like how much
                for (int vi = i + 1; vi < toks.Length; vi++)
                {
                    string tag = toks[vi].GetType().ToString();
                    if (tag != null && tag.StartsWith("V"))
                    {
                        verb = toks[vi].ToString();
                        break;
                    }
                }
                if (focalNoun == null)
                {
                    features.Add("qw_verb=" + queryWord + "_" + verb);
                    features.Add("verb=" + verb);
                    features.Add("fn=null");
                }
                else if (useFocusNounPattern.matcher(queryWord).matches())
                {
                    generateWordFeatures(focalNoun, features);
                    generateWordNetFeatures(focalNoun, features);
                }
                if (fnIsLast)
                {
                    features.Add("fnIsLast=" + fnIsLast);
                }
            }
            return (features.ToArray());
        }
    }
}
