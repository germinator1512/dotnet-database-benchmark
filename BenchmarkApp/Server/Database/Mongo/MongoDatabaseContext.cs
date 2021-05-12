using BenchmarkApp.Server.Database.Entities;
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

        public IMongoCollection<Entity> Entities => _database.GetCollection<Entity>("entities");
    }
}