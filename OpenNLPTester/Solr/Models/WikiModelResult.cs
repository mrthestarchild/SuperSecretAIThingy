using SolrNet.Attributes;
using System.Collections.Generic;

namespace QuestionAnswerAi.Solr.Models
{
    class WikiModelResult : ISolrBaseModel
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("revisionId")]
        public ICollection<string> RevisionId { get; set; }

        [SolrField("title")]
        public ICollection<string> Title { get; set; }

        [SolrField("redirectTitle")]
        public ICollection<string> RedirectTitle { get; set; }

        [SolrField("format")]
        public ICollection<string> Format { get; set; }

        [SolrField("revisionText")]
        public ICollection<string> RevisionText { get; set; }

        [SolrField("parentId")]
        public ICollection<int> ParentId { get; set; }

        [SolrField("sha")]
        public ICollection<string> Sha { get; set; }

        [SolrField("modelType")]
        public ICollection<string> ModelType { get; set; }

        [SolrField("score")]
        public double Score { get; set; }
    }
}
