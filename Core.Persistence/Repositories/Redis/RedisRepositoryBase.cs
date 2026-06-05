using Core.Abstractions.Repositories.Redis;
using Core.Domain;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Persistence.Repositories.Redis
{
    public abstract class RedisRepositoryBase<TEntity> : IRedisRepository<TEntity> where TEntity : IRedisEntity
    {

        private readonly IDatabase _database;

        private readonly JsonSerializerSettings _jsonSerializerSettings;

        protected RedisRepositoryBase(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();

            _jsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
        }


        public async Task SetAsync(TEntity entiy, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            string key = entiy.GetRedisKey();

            string json = JsonConvert.SerializeObject(entiy, _jsonSerializerSettings);

            await _database.StringSetAsync(key, json, expiry);
        }

        public async Task<TEntity?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            RedisValue value = await _database.StringGetAsync(key);

            if (!value.HasValue)
            {
                return default;
            }

            TEntity? entity = JsonConvert.DeserializeObject<TEntity>(value, _jsonSerializerSettings);
            return entity;
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _database.KeyExistsAsync(key);
        }

        public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
        {
            await _database.KeyDeleteAsync(key);
        }



        public async Task<IEnumerable<TEntity>> GetAllAsync(string pattern, int pageSize = 100, CancellationToken cancellationToken = default)
        {
            var server = _database.Multiplexer.GetServer(_database.Multiplexer.GetEndPoints().First());

            var entities = new List<TEntity>();

            foreach (var key in server.Keys(database: _database.Database, pattern: pattern, pageSize: pageSize))
            {
                var entity = await GetAsync(key, cancellationToken);
                if (entity != null)
                {
                    entities.Add(entity);
                }
            }
            return entities;
        }

    }
}
