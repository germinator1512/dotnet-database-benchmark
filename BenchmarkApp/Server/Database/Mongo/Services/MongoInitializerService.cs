using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo.Entities;
using BenchmarkApp.Server.Database.Mongo.Interfaces;
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

            var friends = await repository.GetAllFriendsAsync(6);

            if (!friends.Any())
                await AddDataSet(context);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private async Task AddDataSet(MongoDatabaseContext context)
        {
            Console.WriteLine("No entities found in MongoDB - Inserting Test Dataset");

            var firstUser = new MongoUserEntity
            {
                Name = "Max Mustermann",
            };

            await context.Users.InsertOneAsync(firstUser);

            var level1Friends = GenerateFriends(firstUser, 9, 1);
            // await context.Users.InsertManyAsync(level1Friends);
        }

        // https://chrisbitting.com/2015/03/24/mongodb-linking-records-documents-using-mongodbref/
        private List<MongoUserEntity> GenerateFriends(MongoUserEntity rootFriend,
            int howMany,
            int level)
        {
            var newFriends = new List<MongoUserEntity>();
            for (var z = 1; z <= howMany; z++)
            {
                var friend = new MongoUserEntity
                {
                    Name = $"User {rootFriend.Id} Level {level} Friend {z}",
                };

                // rootFriend.Friends.Add(new MongoDBRef {"", friend.Id});
            }

            return newFriends;
        }
    }
}