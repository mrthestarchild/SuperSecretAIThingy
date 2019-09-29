using SolrNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNLPTester.Solr.Models
{
    // TODO: get rid of this in production usage.
    /// <summary>
    /// techproducts model from Solr Tutorial
    /// </summary>
    class techproducts : ISolrBaseModel
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("links")]
        public ICollection<string> Links { get; set; }

        [SolrField("resourcename")]
        public string ResourceName { get; set; }

        [SolrField("title")]
        public ICollection<string> Title { get; set; }

        [SolrField("content_type")]
        public ICollection<string> ContentType { get; set; }

        [SolrField("content")]
        public ICollection<string> Content { get; set; }

        [SolrField("_version_")]
        public long Version { get; set; }

    }
}
