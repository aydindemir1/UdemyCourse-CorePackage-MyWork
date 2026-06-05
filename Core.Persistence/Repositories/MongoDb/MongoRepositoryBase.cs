using Core.Abstractions.Repositories.MongoDb;
using Core.Domain;
using Core.Persistence.Repositories.MongoDb.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Core.Persistence.Repositories.MongoDb
{
    public abstract class MongoRepositoryBase<TEntity, TEntityId> : IMongoAsyncRepository<TEntity, TEntityId>
    where TEntity : BaseEntity<TEntityId>
    {
        private readonly IMongoCollection<TEntity> _collection;

        protected MongoRepositoryBase(MongoConnectionSettings settings, string collectionName)
        {
            if (string.IsNullOrWhiteSpace(collectionName))
                throw new ArgumentException("Collection name cannot be null");

            var client = settings.GetMongoClientSettings() == null ? new MongoClient(settings.ConnectionString)
                : new MongoClient(settings.GetMongoClientSettings());

            var database = client.GetDatabase(settings.DatabaseName);

            _collection = database.GetCollection<TEntity>(collectionName);
        }

        public IMongoCollection<TEntity> GetCollection()
        {
            return _collection;
        }


        public async Task AddAsync(TEntity entity, InsertOneOptions options = null, CancellationToken cancellationToken = default)
        {
            await _collection.InsertOneAsync(entity, options ?? new InsertOneOptions(), cancellationToken);
        }

        public async Task AddManyAsync(IEnumerable<TEntity> entities, InsertManyOptions options = null, CancellationToken cancellationToken = default)
        {
            await _collection.InsertManyAsync(entities, options ?? new InsertManyOptions(), cancellationToken);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(predicate).AnyAsync(cancellationToken);
        }

        public async Task DeleteAsync(TEntityId id, CancellationToken cancellationToken = default)
        {
            await _collection.DeleteOneAsync(e => e.Id.Equals(id), cancellationToken);
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            await _collection.DeleteManyAsync(predicate, cancellationToken);
        }

        public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, CancellationToken cancellationToken = default)
        {
            return predicate == null ? await _collection.Find(FilterDefinition<TEntity>.Empty).ToListAsync(cancellationToken) : await _collection.Find(predicate).ToListAsync(cancellationToken);
        }

        public async Task<TEntity> GetAsync(TEntityId id, CancellationToken cancellationToken = default)
        {
            return await _collection.Find(e => e.Id.Equals(id)).FirstOrDefaultAsync(cancellationToken);
        }



        public async Task UpdateAsync(TEntityId id, TEntity entity, ReplaceOptions options = null, CancellationToken cancellationToken = default)
        {
            await _collection.ReplaceOneAsync(e => e.Id.Equals(id), entity, options ?? new ReplaceOptions(), cancellationToken);
        }

        public async Task UpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity, ReplaceOptions options = null, CancellationToken cancellationToken = default)
        {
            await _collection.ReplaceOneAsync(predicate, entity, options ?? new ReplaceOptions(), cancellationToken);
        }
    }
}
