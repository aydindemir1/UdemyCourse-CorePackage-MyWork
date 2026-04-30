using Core.Abstractions.Paging;
using Core.Abstractions.Repositories;
using Core.Domain;
using Core.Persistence.Paging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Core.Persistence.Repositories.EfCore
{
    public abstract class EfRepositoryBase<TEntity, TEntityId, TContext> : IRepository<TEntity, TEntityId>, IAsyncRepository<TEntity, TEntityId>
    where TEntity : BaseEntity<TEntityId>
    where TContext : DbContext
    {

        protected readonly TContext Context;

        protected EfRepositoryBase(TContext context)
        {
            Context = context;
        }

        public IQueryable<TEntity> Query() => Context.Set<TEntity>();


        protected IQueryable<TEntity> BuildQuery(Expression<Func<TEntity, bool>> predicate = null, bool withDeleted = false, bool asNoTracking = false, bool useSplitQuery = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null)
        {
            IQueryable<TEntity> queryable = Query();

            if (withDeleted)
                queryable = queryable.IgnoreQueryFilters();

            if (asNoTracking)
                queryable = queryable.AsNoTracking();

            if (useSplitQuery)
                queryable = queryable.AsSplitQuery();

            if (customize != null)
                queryable = customize(queryable);
            if (predicate is not null)
                queryable = queryable.Where(predicate);

            return queryable;
        }


        public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> predicate, bool withDeleted = false, bool asNoTracking = false, bool useSplitQuery = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null, CancellationToken cancellationToken = default)
        {
            var query = BuildQuery(predicate, withDeleted, asNoTracking, useSplitQuery, customize);
            return await query.FirstOrDefaultAsync(cancellationToken);
        }


        public TEntity? Get(Expression<Func<TEntity, bool>> predicate, bool withDeleted = false, bool asNoTracking = false, bool useSplitQuery = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null)
        {
            var query = BuildQuery(predicate, withDeleted, asNoTracking, useSplitQuery, customize);
            return query.FirstOrDefault();
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate = null, bool withDeleted = false, bool asNoTracking = false, bool useSplitQuery = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null, CancellationToken cancellationToken = default)
        {
            var query = BuildQuery(predicate, withDeleted, asNoTracking, useSplitQuery, customize);
            return await query.ToListAsync(cancellationToken);
        }

        public List<TEntity> GetList(Expression<Func<TEntity, bool>> predicate = null, bool withDeleted = false, bool asNoTracking = false, bool useSplitQuery = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null)
        {
            var query = BuildQuery(predicate, withDeleted, asNoTracking, useSplitQuery, customize);
            return query.ToList();
        }

        public async Task<TResult?> GetProjectedAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, bool withDeleted = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null, CancellationToken cancellationToken = default)
        {
            var query = BuildQuery(predicate, withDeleted, customize: customize);
            return await query.Select(selector).FirstOrDefaultAsync(cancellationToken);
        }


        public TResult? GetProjected<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, bool withDeleted = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null)
        {
            var query = BuildQuery(predicate, withDeleted, customize: customize);
            return query.Select(selector).FirstOrDefault();
        }


        public Task<List<TResult>> GetListProjectedAsync<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, bool withDeleted = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null, CancellationToken cancellationToken = default)
        {
            var query = BuildQuery(predicate, withDeleted, customize: customize);
            return query.Select(selector).ToListAsync(cancellationToken);
        }

        public List<TResult> GetListProjected<TResult>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TResult>> selector, bool withDeleted = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null)
        {
            var query = BuildQuery(predicate, withDeleted, customize: customize);
            return query.Select(selector).ToList();
        }

        public async Task<IPaginate<TEntity>> GetPaginatedAsync(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize, bool withDeleted = false, bool asNoTracking = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null, CancellationToken cancellationToken = default)
        {
            var query = BuildQuery(predicate, withDeleted, asNoTracking, customize: customize);
            return await query.ToPaginateAsync(pageIndex, pageSize, cancellationToken);
        }


        public IPaginate<TEntity> GetPaginated(Expression<Func<TEntity, bool>> predicate, int pageIndex, int pageSize, bool withDeleted = false, bool asNoTracking = false, Func<IQueryable<TEntity>, IQueryable<TEntity>>? customize = null)
        {
            var query = BuildQuery(predicate, withDeleted, asNoTracking, customize: customize);
            return query.ToPaginate(pageIndex, pageSize);
        }


        public async Task<TEntity> AddAsync(TEntity entity)
        {
            entity.CreatedDate = DateTimeOffset.UtcNow;
            await Context.AddAsync(entity);
            return entity;
        }


        public TEntity Add(TEntity entity)
        {
            entity.CreatedDate = DateTimeOffset.UtcNow;
            Context.Add(entity);
            return entity;
        }

        public async Task<ICollection<TEntity>> AddRangeAsync(ICollection<TEntity> entities)
        {
            foreach (TEntity entity in entities)
                entity.CreatedDate = DateTimeOffset.UtcNow;
            await Context.AddRangeAsync(entities);
            return entities;
        }


        public ICollection<TEntity> AddRange(ICollection<TEntity> entities)
        {
            foreach (TEntity entity in entities)
                entity.CreatedDate = DateTimeOffset.UtcNow;
            Context.AddRange(entities);
            return entities;
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate = null, bool withDeleted = false, CancellationToken cancellationToken = default)
        {
            var query = BuildQuery(predicate, withDeleted);
            return await query.AnyAsync(cancellationToken);
        }

        public bool Any(Expression<Func<TEntity, bool>> predicate = null, bool withDeleted = false)
        {
            var query = BuildQuery(predicate, withDeleted);
            return query.Any();
        }

        public async Task<TEntity> DeleteAsync(TEntity entity, bool permanent = false, CancellationToken cancellationToken = default)
        {
            await SetEntityAsDeletedAsync(entity, permanent, cancellationToken);
            return entity;
        }

        public TEntity Delete(TEntity entity, bool permanent = false)
        {
            SetEntityAsDeleted(entity, permanent);
            return entity;
        }

        public async Task<ICollection<TEntity>> DeleteRangeAsync(ICollection<TEntity> entities, bool permanent = false, CancellationToken cancellationToken = default)
        {
            foreach (TEntity entity in entities)
                await SetEntityAsDeletedAsync(entity, permanent, cancellationToken);
            return entities;
        }

        public ICollection<TEntity> DeleteRange(ICollection<TEntity> entities, bool permanent = false)
        {
            foreach (TEntity entity in entities)
                SetEntityAsDeleted(entity, permanent);
            return entities;
        }

        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            entity.UpdatedDate = DateTimeOffset.UtcNow;
            Context.Update(entity);
            return Task.FromResult(entity);
        }


        public TEntity Update(TEntity entity)
        {
            entity.UpdatedDate = DateTimeOffset.UtcNow;
            Context.Update(entity);
            return entity;
        }

        public Task<ICollection<TEntity>> UpdateRangeAsync(ICollection<TEntity> entities)
        {
            foreach (TEntity entity in entities)
                entity.UpdatedDate = DateTimeOffset.UtcNow;
            Context.UpdateRange(entities);
            return Task.FromResult(entities);
        }


        public ICollection<TEntity> UpdateRange(ICollection<TEntity> entities)
        {
            foreach (TEntity entity in entities)
                entity.UpdatedDate = DateTimeOffset.UtcNow;
            Context.UpdateRange(entities);
            return entities;
        }




        private void SoftDeleteEntityGraph(TEntity entity)
        {
            SoftDeleteEntity(entity);

            var navigations = Context.Entry(entity)
                              .Metadata.GetNavigations()
                              .Where(n => n.IsCollection &&
                                (n.ForeignKey.DeleteBehavior == DeleteBehavior.ClientCascade ||
                                 n.ForeignKey.DeleteBehavior == DeleteBehavior.Cascade)).ToList();

            foreach (var navigation in navigations)
            {
                if (navigation.PropertyInfo == null) continue;

                object? navValue = navigation.PropertyInfo.GetValue(entity);

                if (navValue == null)
                {
                    var query = navigation.IsCollection
                        ? Context.Entry(entity).Collection(navigation.Name).Query()
                        : Context.Entry(entity).Reference(navigation.Name).Query();

                    navValue = navigation.IsCollection
                        ? GetRelationLoaderQuery(query, navigation.ClrType).ToList()
                        : GetRelationLoaderQuery(query, navigation.ClrType).FirstOrDefault();
                }

                if (navigation.IsCollection && navValue is ICollection collection)
                {
                    foreach (var item in collection) { SoftDeleteEntity(item); }
                }
                else if (navValue != null)
                {
                    SoftDeleteEntity(navValue);
                }
            }
        }

        // [GÜNCELLEME]: Eğitimdeki SoftDeleteEntityAsync metodu kaldırıldı. 
        // Daha performanslı ve 'Cascade' mantığını tam asenkron destekleyen bu yapıya geçildi.
        private async Task SoftDeleteEntityGraphAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            SoftDeleteEntity(entity);

            var navigations = Context.Entry(entity)
                              .Metadata.GetNavigations()
                              .Where(n => n.IsCollection && // Burada navigation.IsCollection kontrolü eklenerek, 
                                                            // sadece koleksiyon tipindeki (Bire-Çok / Çok-a-Çok) ilişkilerin silinmesi garanti altına alındı.
                                                            // Örnek: RefreshToken silinirken bağlı olduğu User'ın (Reference) silinmesi bu kontrolle engellenir.
                                (n.ForeignKey.DeleteBehavior == DeleteBehavior.ClientCascade ||
                                 n.ForeignKey.DeleteBehavior == DeleteBehavior.Cascade)).ToList();

            foreach (var navigation in navigations)
            {
                if (navigation.PropertyInfo == null) continue;

                object? navValue = navigation.PropertyInfo.GetValue(entity);

                if (navValue == null)
                {
                    var query = navigation.IsCollection
                        ? Context.Entry(entity).Collection(navigation.Name).Query()
                        : Context.Entry(entity).Reference(navigation.Name).Query();

                    var relationQuery = GetRelationLoaderQuery(query, navigation.ClrType);

                    navValue = navigation.IsCollection
                        ? await relationQuery.ToListAsync(cancellationToken)
                        : await relationQuery.FirstOrDefaultAsync(cancellationToken);
                }

                if (navigation.IsCollection && navValue is ICollection collection)
                {
                    foreach (var item in collection) { SoftDeleteEntity(item); }
                }
                else if (navValue != null)
                {
                    SoftDeleteEntity(navValue);
                }
            }
        }

        private void SoftDeleteEntity(object entity)
        {
            var deletedDateProperty = entity.GetType().GetProperty("DeletedDate");

            //DateTime yerine DateTimeOffset? kontrolü yapıyoruz. 
            // Mikroservis mimarilerinde farklı zaman dilimleriyle (Timezone) başa çıkabilmek için 
            // profesyonel yaklaşım DateTimeOffset kullanmaktır.
            if ((deletedDateProperty == null || deletedDateProperty.PropertyType != typeof(DateTimeOffset?)))
            {
                return;
            }

            var currentValue = deletedDateProperty.GetValue(entity) as DateTimeOffset?;
            if (currentValue.HasValue) return;

            deletedDateProperty.SetValue(entity, DateTimeOffset.UtcNow);

            Context.Update(entity);
        }

        private void SetEntityAsDeleted(object entity, bool permanent)
        {
            if (permanent)
            {
                Context.Remove(entity);
            }
            else
            {
                SoftDeleteEntityGraph((TEntity)entity);
            }
        }

        private async Task SetEntityAsDeletedAsync(object entity, bool permanent, CancellationToken cancellationToken = default)
        {
            if (permanent)
            {
                Context.Remove(entity);
            }
            else
            {
                await SoftDeleteEntityGraphAsync((TEntity)entity, cancellationToken);
            }
        }


        protected IQueryable<object> GetRelationLoaderQuery(IQueryable query, Type navPropertyType)
        {
            Type queryProviderType = query.Provider.GetType();
            MethodInfo? createQueryMethod = queryProviderType.GetMethods().FirstOrDefault(m => m.Name == nameof(query.Provider.CreateQuery) && m.IsGenericMethod && m.GetParameters().Length == 1);

            if (createQueryMethod == null)
                throw new InvalidOperationException("CreateQuery<T>() method not found in Ef Core");

            MethodInfo genericMethod = createQueryMethod.MakeGenericMethod(navPropertyType);

            var rawQuery = genericMethod.Invoke(query.Provider, new object[] { query.Expression });

            if (rawQuery is not IQueryable<object> objectQueryable)
                throw new InvalidOperationException("Query could not be cast to correct type in Ef Core");
            return objectQueryable.IgnoreQueryFilters();
        }
    }
}
