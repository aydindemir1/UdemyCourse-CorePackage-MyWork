using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Paging
{
    public interface IPaginate<T>
    {

        int PageIndex { get; }

        int PageSize { get; }

        int TotalCount { get; }

        int TotalPages { get; }

        IList<T> Items { get; }

        bool HasPrevious { get; }

        bool HasNext { get; }

    }
}
