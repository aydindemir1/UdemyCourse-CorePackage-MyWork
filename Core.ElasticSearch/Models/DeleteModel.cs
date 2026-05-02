using System;
using System.Collections.Generic;
using System.Text;

namespace Core.ElasticSearch.Models
{
    public class DeleteModel
    {
        public string IndexName { get; set; } = string.Empty;
        public string DocumentId { get; set; } = string.Empty;
    }
}
