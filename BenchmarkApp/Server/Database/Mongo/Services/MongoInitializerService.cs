using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
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
        private MongoDatabaseContext _context;

        public MongoInitializerService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            _context = scope.ServiceProvider.GetRequiredService<MongoDatabaseContext>();
            var repository = scope.ServiceProvider.GetRequiredService<IMongoRepository>();

            await repository.EmptyDatabase(cancellationToken);

            if (await repository.IsDatabaseEmpty(cancellationToken))
                await AddDataSet();
        }


        private async Task AddDataSet()
        {
            Console.WriteLine("No entities found in MongoDB - Inserting Test Dataset");

            var firstUser = new MongoUserEntity
            {
                Name = Config.RootUserName,
            };

            await _context.Users.InsertOneAsync(firstUser);
            await AddFriendRecursively(firstUser, Config.FriendsPerUser - 1, Config.NestedUserLevels);
        }

        /// <summary>
        /// adds nested user entities to database until given level is reached
        /// </summary>
        /// <param name="root">first user entity</param>
        /// <param name="howMany">number of new entities to be added to friends list of root user</param>
        /// <param name="nestedLevels">how many levels deep entities should be nested e.g 6 adds 10^6 users</param>
        /// <param name="currentLevel">current level of function call</param>
        private async Task AddFriendRecursively(
            MongoUserEntity root,
            int howMany,
            int nestedLevels,
            int currentLevel = 1)
        {
            List<MongoUserEntity> GenerateFriends() => Enumerable
                .Range(1, howMany)
                .Select(z => new MongoUserEntity {Name = Config.UserName(currentLevel, z),})
                .ToList();

            var friends = GenerateFriends();
            await _context.Users.InsertManyAsync(friends);

            List<MongoFriendShipEntity> GenerateFriendShips()
                => friends.Select(f => new MongoFriendShipEntity
                    {
                        FriendARef = new MongoDBRef("users", root.Id),
                        FriendBRef = new MongoDBRef("users", f.Id),
                    })
                    .ToList();

            var friendships = GenerateFriendShips();

            await _context.FriendShips.InsertManyAsync(friendships);

            if (currentLevel < nestedLevels)
                foreach (var friend in friends)
                    await AddFriendRecursively(friend, Config.FriendsPerUser, nestedLevels, currentLevel + 1);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}