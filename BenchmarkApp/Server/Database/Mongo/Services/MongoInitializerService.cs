using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo.Entities;
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
            var context = scope.ServiceProvider.GetRequiredService<MongoDatabaseContext>();

            var friends = await context.Users.Find(_ => true).ToListAsync(cancellationToken);
            if (!friends.Any()) await AddDataSet(context);
        }


        /// <summary>
        /// adds 1 million users with nested friendships to mongo database 
        /// </summary>
        /// <param name="context"></param>
        private static async Task AddDataSet(MongoDatabaseContext context)
        {
            Console.WriteLine("No entities found in MongoDB - Inserting Test Dataset");

            var users = context.Users;
            var friendships = context.FriendShips;

            var firstUser = new MongoUserEntity
            {
                Name = "Max Mustermann",
            };

            await users.InsertOneAsync(firstUser);
            await AddFriendRecursively(context, firstUser, 9, 3, 0);
        }

        /// <summary>
        /// adds nested user entities to database until given level is reached
        /// </summary>
        /// <param name="context">database context to add entities to</param>
        /// <param name="root">first user entity</param>
        /// <param name="howMany">number of new entities to be added</param>
        /// <param name="level">how many levels deep entities should be nested</param>
        /// <param name="currentDepth">current level of function call</param>
        private static async Task AddFriendRecursively(
            MongoDatabaseContext context,
            MongoUserEntity root,
            int howMany,
            int level,
            int currentDepth)
        {
            var friends = GenerateFriends(howMany, currentDepth);
            await context.Users.InsertManyAsync(friends);

            var friendships = GenerateFriendShips(root, friends);
            await context.FriendShips.InsertManyAsync(friendships);

            if (level > currentDepth)
            {
                foreach (var friend in friends)
                {
                    await AddFriendRecursively(context, friend, 10, level, ++currentDepth);
                }
            }
        }


        private static List<MongoUserEntity> GenerateFriends(
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

                newFriends.Add(friend);
            }

            return newFriends;
        }

        private static IEnumerable<MongoFriendShipEntity> GenerateFriendShips(
            MongoUserEntity rootFriend,
            IEnumerable<MongoUserEntity> friends)
        {
            var newFriends = new List<MongoFriendShipEntity>();
            foreach (var friend in friends)
            {
                var friendShip = new MongoFriendShipEntity
                {
                    FriendARef = new MongoDBRef("users", rootFriend.Id),
                    FriendBRef = new MongoDBRef("users", friend.Id),
                };

                newFriends.Add(friendShip);
            }

            return newFriends;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}