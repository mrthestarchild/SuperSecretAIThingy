using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLPTester.Solr.Models
{
    class SolrBaseModel : ISolrBaseModel
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }
    }
}
