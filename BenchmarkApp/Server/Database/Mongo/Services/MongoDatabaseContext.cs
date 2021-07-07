using BenchmarkApp.Server.Database.Mongo.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Services
{
    public class MongoDatabaseContext
    {
        public readonly IMongoDatabase Database;

        public MongoDatabaseContext(IConfiguration configuration)
        {
            var config = new MongoConfig();
            configuration.GetSection("MongoConfig").Bind(config);

            var client = new MongoClient(config.DatabaseConnectionString);
            Database = client.GetDatabase(config.DatabaseName);
        }

        public IMongoCollection<MongoUserEntity> Users => Database.GetCollection<MongoUserEntity>("users");

        public IMongoCollection<MongoFriendShipEntity> FriendShips => Database.GetCollection<MongoFriendShipEntity>("friendShips");
    }
}