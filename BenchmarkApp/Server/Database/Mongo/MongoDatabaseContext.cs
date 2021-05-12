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
            var connectionString = configuration.GetValue<string>("DatabaseConnectionString");
            var databaseName = configuration.GetValue<string>("DatabaseName");

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<Entity> Entities => _database.GetCollection<Entity>("entities");
    }
}