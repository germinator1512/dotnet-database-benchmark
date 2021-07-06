using BenchmarkApp.Server.Database.Mongo.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Services
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

        public IMongoCollection<MongoUserEntity> Users => _database.GetCollection<MongoUserEntity>("users");

        public IMongoCollection<MongoUserEntity> FriendShips => _database.GetCollection<MongoUserEntity>("friendShips");
    }
}