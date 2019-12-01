using SolrNet.Attributes;
using System.Collections.Generic;

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
