using SolrNet.Attributes;

namespace QuestionAnswerAi.Solr.Models
{
    class SolrBaseModel : ISolrBaseModel
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }
    }
}
