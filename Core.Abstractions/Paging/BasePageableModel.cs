using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Paging
{
    public abstract class BasePageableModel
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public bool HasPrevious { get; set; }

        public bool HasNext { get; set; }
    }
}
