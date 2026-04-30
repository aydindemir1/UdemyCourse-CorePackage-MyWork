using Core.Abstractions.Paging;
using Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.Abstractions.Repositories
{
    public interface IAsyncRepository<TEntity, TEntityId> : IQuery<TEntity>
    where TEntity : BaseEntity<TEntityId>
    {

        Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, bool withDeleted = false, bool asNoTracking = false, bool useSplitQuery = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null, CancellationToken cancellationToken = default);

        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null, bool withDeleted = false, bool asNoTracking = false, bool useSplitQuery = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null, CancellationToken cancellationToken = default);

        Task<IPaginate<TEntity>> GetPaginatedAsync(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize, bool withDeleted = false, bool asNoTracking = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null, CancellationToken cancellationToken = default);


        Task<TResult?> GetProjectedAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, bool withDeleted = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null, CancellationToken cancellationToken = default);


        Task<List<TResult>> GetListProjectedAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, bool withDeleted = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null, CancellationToken cancellationToken = default);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null, bool withDeleted = false, CancellationToken cancellationToken = default);

        Task<TEntity> AddAsync(TEntity entity);
        Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities);

        Task<TEntity> UpdateAsync(TEntity entity);
        Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities);

        Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false, CancellationToken cancellationToken = default);
        Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false, CancellationToken cancellationToken = default);

    }
}
