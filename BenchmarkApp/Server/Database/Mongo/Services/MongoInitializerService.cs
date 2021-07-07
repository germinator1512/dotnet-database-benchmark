using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo.Entities;
using BenchmarkApp.Server.Database.Mongo.Interfaces;
using BenchmarkApp.Server.Database.SQL.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Services
{
    public class MongoInitializerService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public MongoInitializerService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IMongoRepository>();
            var context = scope.ServiceProvider.GetRequiredService<MongoDatabaseContext>();

            var friends = new List<MongoUserEntity>();

            if (!friends.Any()) await AddDataSet(context);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task AddDataSet(MongoDatabaseContext context)
        {
            Console.WriteLine("No entities found in MongoDB - Inserting Test Dataset");

            var users = context.Database.GetCollection<MongoUserEntity>("users");
            var friendships = context.Database.GetCollection<MongoFriendShipEntity>("friendShips");

            var firstUser = new MongoUserEntity
            {
                Name = "Max Mustermann",
            };

            var level1Friends = await AddFriends(firstUser, users, friendships, 9, 1);
            await context.Users.InsertOneAsync(firstUser);

            foreach (var level1Friend in level1Friends)
            {
                var level2Friends = await AddFriends(level1Friend, users, friendships, 10, 2);
            }
        }

        private async Task<List<MongoUserEntity>> AddFriends(MongoUserEntity rootFriend,
            IMongoCollection<MongoUserEntity> users,
            IMongoCollection<MongoFriendShipEntity> friendships,
            int howMany,
            int level)
        {
            var newFriends = new List<MongoUserEntity>();
            for (var z = 1; z <= howMany; z++)
            {
                var friend = new MongoUserEntity
                {
                    Name = $"Level {level} Friend {z}",
                };

                await users.InsertOneAsync(friend);

                var friendShip = new MongoFriendShipEntity
                {
                    FriendRef = new MongoDBRef("users", friend.Id),
                };
                await friendships.InsertOneAsync(friendShip);

                var filter = Builders<MongoUserEntity>.Filter.Eq(x => x.Id, rootFriend.Id);
                var dbRef = new MongoDBRef("friendShips", friendShip.Id);
                var updateDefinition = Builders<MongoUserEntity>.Update.AddToSet(u => u.FriendShipRefs, dbRef);
                await users.UpdateOneAsync(filter, updateDefinition);

                newFriends.Add(friend);
            }


            return newFriends;
        }
    }
}