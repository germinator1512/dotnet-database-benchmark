using BenchmarkApp.Server.Database.SQL.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo
{
    public class MongoDatabaseContext
    {
        private readonly IMongoDatabase _database;

        public MongoDatabaseContext(IConfiguration configuration)
        {
            var config = new MongoConfig();
            configuration.GetSection("MongoConfig").Bind(config);

            var client = new MongoClient(config.DatabaseConnectionString);
            _database = client.GetDatabase(config.DatabaseName);
        }

        public IMongoCollection<UserEntity> Users => _database.GetCollection<UserEntity>("users");
    }
}