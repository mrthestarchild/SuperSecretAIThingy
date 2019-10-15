using CommonServiceLocator;
using OpenNLPTester.Models;
using OpenNLPTester.Solr.Models;
using SolrNet;
using SolrNet.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLPTester.Solr
{
    class SolrQueryMethods<T> where T : ISolrBaseModel
    {
        private ISolrOperations<T> solr;

        public SolrQueryMethods(ISolrOperations<T> _solr){
            solr = _solr;
        }
        
        // TODO: make generic
        public SolrQueryResults<T> QueryList(QuestionParserResponseModel parseQuestionObj, string fieldToSearch, int numberOfRows = 10)
        {
            return solr.Query(new SolrQueryInList(fieldToSearch, parseQuestionObj.QueryParams), new QueryOptions
            {
                StartOrCursor = new StartOrCursor.Start(0),
                Rows = numberOfRows,
                ExtraParams = new Dictionary<string, string> {
                { "fl", "*,score" }
            }
            });
        }

        private PropertyInfo[] GetProperties(T t)
        {
            return t.GetType().GetProperties();
        }
    }
}
