using Core.Domain;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.Abstractions.Repositories.MongoDb
{
    public interface IMongoAsyncRepository<TEntity, TEntityId> : IMongoBaseRepository<TEntity>
    where TEntity : BaseEntity<TEntityId>
    {
        Task AddAsync(TEntity entity, InsertOneOptions options = null, CancellationToken cancellationToken = default);
        Task AddManyAsync(IEnumerable<TEntity> entities, InsertManyOptions options = null, CancellationToken cancellationToken = default);

        Task<TEntity> GetAsync(TEntityId id, CancellationToken cancellationToken = default);
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        Task UpdateAsync(TEntityId id, TEntity entity, ReplaceOptions options = null, CancellationToken cancellationToken = default);
        Task UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity, ReplaceOptions options = null, CancellationToken cancellationToken = default);

        Task DeleteAsync(TEntityId id, CancellationToken cancellationToken = default);
        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    }
}
