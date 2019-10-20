﻿using CommonServiceLocator;
using QuestionAnswerAi.Models;
using QuestionAnswerAi.Solr.Models;
using SolrNet;
using SolrNet.Commands.Parameters;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace QuestionAnswerAi.Solr
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
    }
}
