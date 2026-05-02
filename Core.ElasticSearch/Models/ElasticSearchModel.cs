using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ElasticSearch.Models
{
    public class ElasticSearchModel
    {
        public string IndexName { get; set; }

        public string ElasticId { get; set; }

        public ElasticSearchModel()
        {
            IndexName = string.Empty;
            ElasticId = string.Empty;
        }

        public ElasticSearchModel(string indexName, string elasticId)
        {
            IndexName = indexName;
            ElasticId = elasticId;
        }
    }
}
