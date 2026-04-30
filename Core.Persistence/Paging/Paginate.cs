using Core.Abstractions.Paging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Persistence.Paging
{
    public class Paginate<T> : IPaginate<T>
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }

        public IList<T> Items { get; set; }

        public bool HasPrevious => PageIndex > 1;

        public bool HasNext => PageIndex < TotalPages;

        public Paginate(int totalCount, int pageIndex, int pageSize, IList<T> items)
        {
            Items = items;
            TotalCount = totalCount;
            PageIndex = pageIndex;
            PageSize = pageSize;


            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        }
    }
}
