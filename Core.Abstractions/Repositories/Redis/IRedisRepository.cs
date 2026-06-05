using Core.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Repositories.Redis
{
    public interface IRedisRepository<TEntiy> where TEntiy : IRedisEntity
    {
        Task SetAsync(TEntiy entiy, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
        Task<TEntiy?> GetAsync(string key, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
        Task DeleteAsync(string key, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntiy>> GetAllAsync(string pattern, int pageSize = 100, CancellationToken cancellationToken = default);
    }
}
