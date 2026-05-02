using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ElasticSearch.Models
{
    public class SearchParameters
    {
        public string IndexName { get; set; }

        public int From { get; set; } = 0;

        public int Size { get; set; } = 10;

        public string Keyword { get; set; } = string.Empty;

        public SearchParameters()
        {
            IndexName = string.Empty;
        }

        public SearchParameters(string indexName, int from, int size)
        {
            IndexName = indexName;
            From = from;
            Size = size;
        }
    }
}
