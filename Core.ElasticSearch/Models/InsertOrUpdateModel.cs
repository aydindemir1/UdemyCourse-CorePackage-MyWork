using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ElasticSearch.Models
{
    public class InsertOrUpdateModel : ElasticSearchModel
    {
        public object Item { get; set; }

        public InsertOrUpdateModel(object item)
        {
            Item = item;
        }

        public InsertOrUpdateModel(string elasticId, string indexName, object item) : base(indexName, elasticId)
        {
            Item = item;
        }
    }
}
