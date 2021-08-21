using System;
using BenchmarkApp.Server.Database.Mongo.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace BenchmarkApp.Server.Database.Mongo.Services
{
    public class MongoDatabaseContext
    {
        private readonly IMongoDatabase _database;

        public MongoDatabaseContext(IConfiguration configuration)
        {
            var config = new MongoConfig();
            configuration.GetSection("MongoConfig").Bind(config);

            // var client = new MongoClient(config.DatabaseConnectionString);
            var mongoConnectionUrl = new MongoUrl(config.DatabaseConnectionString);
            var mongoClientSettings = MongoClientSettings.FromUrl(mongoConnectionUrl);
            mongoClientSettings.ClusterConfigurator = cb =>
            {
                cb.Subscribe<CommandStartedEvent>(e =>
                {
                   Console.WriteLine($"{e.CommandName} - {e.Command.ToJson()}");
                });
            };
            
            var client = new MongoClient(mongoClientSettings);
            _database = client.GetDatabase(config.DatabaseName);
        }

        public IMongoCollection<MongoUserEntity> Users => _database.GetCollection<MongoUserEntity>("users");

        public IMongoCollection<MongoUserEntity> WriteUsers => _database.GetCollection<MongoUserEntity>("writeUsers");
    }
}