using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Abstractions.Repositories.MongoDb
{
    public interface IMongoBaseRepository<TEntity>
    {
        IMongoCollection<TEntity> GetCollection();
    }
}
