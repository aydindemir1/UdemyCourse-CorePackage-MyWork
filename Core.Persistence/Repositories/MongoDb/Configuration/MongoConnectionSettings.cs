using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Persistence.Repositories.MongoDb.Configuration
{
    public class MongoConnectionSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        public MongoConnectionSettings()
        {

        }

        public MongoConnectionSettings(MongoClientSettings mongoClientSettings)
        {
            MongoClientSettings = mongoClientSettings;
        }

        public MongoClientSettings MongoClientSettings { get; set; }

        public MongoClientSettings GetMongoClientSettings()
        {
            return MongoClientSettings;
        }
    }
}
