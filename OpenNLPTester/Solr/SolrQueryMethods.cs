using CommonServiceLocator;
using OpenNLPTester.Solr.Models;
using SolrNet;
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
        private ISolrBaseModel _t;
        private ISolrConnection solr;

        SolrQueryMethods(ISolrConnection _solr){
            solr = _solr;
        }
        public void DeleteAll()
        {
            DeleteParameters del = new DeleteParameters();
            SolrNet.Impl.FieldSerializers.DefaultFieldSerializer deffield = new SolrNet.Impl.FieldSerializers.DefaultFieldSerializer();
            SolrNet.Impl.QuerySerializers.DefaultQuerySerializer defquery = new SolrNet.Impl.QuerySerializers.DefaultQuerySerializer(deffield);

            SolrNet.Commands.Parameters.DeleteByIdAndOrQueryParam delpar = new SolrNet.Commands.Parameters.DeleteByIdAndOrQueryParam(Enumerable.Empty<string>(), SolrQuery.All, defquery);
            var delete = new SolrNet.Commands.DeleteCommand(delpar, del);
            string res = delete.Execute(solr);
            System.Diagnostics.Trace.WriteLine(res);
        }

        public void CommitChanges()
        {
            SolrNet.Commands.CommitCommand commit = new SolrNet.Commands.CommitCommand();
            string res = commit.Execute(solr);
            System.Diagnostics.Trace.WriteLine(res);
        }

        public void Add(Dictionary<T,double?> dict)
        {
            AddParameters par = new AddParameters();
            ISolrDocumentSerializer<T> ser = ServiceLocator.Current.GetInstance<ISolrDocumentSerializer<T>>();
            SolrNet.Commands.AddCommand<T> add = new SolrNet.Commands.AddCommand<T>(dict, ser, par);
            string res = add.Execute(solr);
            System.Diagnostics.Trace.WriteLine(res);
        }

        public void QueryAll()
        {
            // Query all documents
            var query = SolrQuery.All;
            var operations = ServiceLocator.Current.GetInstance<ISolrOperations<T>>();
            var objects = operations.Query(query);
            var properties = GetProperties(objects[0]);

            int i = 0;
            foreach (var obj in objects)
            {
                foreach(var prop in properties)
                {
                    Console.Write($"{i} {prop.GetValue(obj, null)}");
                }
                i++;
            }

            if (i == 0)
            {
                Console.WriteLine(" = no documents =");
            }
        }

        private PropertyInfo[] GetProperties(T t)
        {
            return t.GetType().GetProperties();
        }
    }
}
