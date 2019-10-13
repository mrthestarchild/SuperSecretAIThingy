using com.google.protobuf;
using java.io;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLPTester.Utils
{
    class AnswerTypeEventStream
    {
        protected BufferedReader reader;
        protected string line;
        protected AnswerTypeContextGenerator atcg;
        protected Parser parser;

        public AnswerTypeEventStream(string fileName, string encoding, AnswerTypeContextGenerator atcg, Parser parser)
        {
            if (encoding == null)
            {
                reader = new BufferedReader(new FileReader(fileName));
            }
            else
            {
                reader = new BufferedReader(new InputStreamReader(new FileInputStream(fileName), encoding));
            }
            this.atcg = atcg;
            this.parser = parser;
        }

        public AnswerTypeEventStream(string fileName, AnswerTypeContextGenerator atcg, Parser parser)
        {

        }

        /**
         * Creates a new file event stream from the specified file.
         * @param file the file containing the events.
         * @throws IOException When the specified file can not be read.
         */
        public AnswerTypeEventStream(File file)
        {
            reader = new BufferedReader(new InputStreamReader(new FileInputStream(file), "UTF8"));
        }

        public bool hasNext()
        {
            try
            {
                return (null != (line = reader.readLine()));
            }
            catch (IOException e)
            {
                System.Console.WriteLine(e);
                return (false);
            }
        }
    }
}

    //    public Event next()
    //    {
    //        int split = line.IndexOf(' ');
    //        string outcome = line.Substring(0, split);
    //        string question = line.Substring(split + 1);
    //        OpenNLP.Tools.Parser.Parse query = 
    //        return (new Event(outcome, atcg.getContext(query)));
    //    }

    //    /**
    //     * Generates a string representing the specified event.
    //     * @param event The event for which a string representation is needed.
    //     * @return A string representing the specified event.
    //     */
    //    public static string toLine(Event event)
    //    {
    //        StringBuffer sb = new StringBuffer();
    //        sb.append(event.getOutcome());
    //        string[] context = event.getContext();
    //        for (int ci = 0; ci < contextLength; ci++)
    //        {
    //            sb.append(" " + context[ci]);
    //        }
    //        sb.append(System.getProperty("line.separator"));
    //        return sb.toString();
    //    }

    //    public static void main(string[] args)
    //    {
    //        if (args.length == 0) {
    //            System.err.println("Usage: AnswerTypeEventStream eventfile");
    //            System.exit(1);
    //        }
    //        int ai=0;
    //        String eventFile = args [ai++];
    //        String modelsDirProp = System.getProperty("models.dir", "book/src/main" + File.separator + "opennlp-models" +
    //            File.separator + "english");
    //        File modelsDir = new File(modelsDirProp);
    //        File wordnetDir = new File(System.getProperty("wordnet.dir", "book/src/main" + File.separator + "WordNet-3.0" + File.separator + "dict"));
    //        InputStream chunkerStream = new FileInputStream(
    //            new File(modelsDir, "en-chunker.bin"));
    //        ChunkerModel chunkerModel = new ChunkerModel(chunkerStream);
    //        ChunkerME chunker = new ChunkerME(chunkerModel);
    //        InputStream posStream = new FileInputStream(
    //            new File(modelsDir, "en-pos-maxent.bin"));
    //        POSModel posModel = new POSModel(posStream);
    //        POSTaggerME tagger = new POSTaggerME(posModel);
    //        Parser parser = new ChunkParser(chunker, tagger);
    //        AnswerTypeContextGenerator actg = new AnswerTypeContextGenerator(wordnetDir);
    //        EventStream es = new AnswerTypeEventStream(eventFile, actg, parser);
    //        while(es.hasNext()) {
    //          System.out.println(es.next().toString());
    //        }
    //      }
    //}
