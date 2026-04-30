using Core.Abstractions.Paging;
using Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.Abstractions.Repositories
{
    public interface IRepository<TEntity, TEntityId> : IQuery<TEntity>
     where TEntity : BaseEntity<TEntityId>
    {

        TEntity? Get(Expression<Func<TEntity, bool>> predicate, bool withDeleted = false, bool asNoTracking = false, bool useSplitQuery = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null);

        List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate = null, bool withDeleted = false, bool asNoTracking = false, bool useSplitQuery = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null);

        IPaginate<TEntity> GetPaginated(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize, bool withDeleted = false, bool asNoTracking = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null);


        TResult? GetProjected<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, bool withDeleted = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null);


        List<TResult> GetListProjected<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, bool withDeleted = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null);

        bool Any(Expression<Func<TEntity, bool>> predicate = null, bool withDeleted = false);

        TEntity Add(TEntity entity);
        ICollection<TEntity> AddRange(ICollection<TEntity> entities);

        TEntity Update(TEntity entity);
        ICollection<TEntity> UpdateRange(ICollection<TEntity> entities);

        TEntity Delete(TEntity entity, bool permanent = false);
        ICollection<TEntity> DeleteRange(ICollection<TEntity> entities, bool permanent = false);

    }
}
