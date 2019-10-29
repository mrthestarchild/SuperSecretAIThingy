using QuestionAnswerAi.Solr.Models;
using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestionAnswerAi.Solr.Models
{
    class QASettingsModel : ISolrBaseModel
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("questionIdentifier")]
        public string QuestionIdentifier { get; set; }

        [SolrField("nerTypes")]
        public List<string> NerTypes { get; set; }
    }
}
