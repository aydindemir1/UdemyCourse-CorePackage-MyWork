using Core.Abstractions.Paging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Persistence.Paging
{
    public static class IQueryablePaginateExtensions
    {

        public static async Task<IPaginate<T>> ToPaginateAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            var count = await source.CountAsync(cancellationToken);

            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return new Paginate<T>(count, pageIndex, pageSize, items);
        }

        public static IPaginate<T> ToPaginate<T>(this IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();

            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new Paginate<T>(count, pageIndex, pageSize, items);
        }
    }
}
