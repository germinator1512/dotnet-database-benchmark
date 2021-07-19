using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Neo4J.Entities;
using BenchmarkApp.Server.Database.Neo4J.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo4jClient;

namespace BenchmarkApp.Server.Database.Neo4J.Services
{
    public class Neo4JInitializerService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private INeo4JRepository _repository;

        public Neo4JInitializerService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            _repository = scope.ServiceProvider.GetRequiredService<INeo4JRepository>();

            var client = scope.ServiceProvider.GetRequiredService<IGraphClient>();
            await client.ConnectAsync();

            // await _repository.EmptyDatabase();

            var isEmpty = await _repository.IsDatabaseEmpty();
            if (isEmpty) await AddDataSet();
        }


        private async Task AddDataSet()
        {
            Console.WriteLine("No entities found in Neo4J - Inserting Test Dataset");

            const string firstName = Config.RootUserName;
            var firstUser = new Neo4JUserEntity
            {
                Name = firstName,
                Id = Guid.NewGuid().ToString()
            };

            await _repository.InsertSingleUser(firstUser);

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
            Neo4JUserEntity root,
            int howMany,
            int nestedLevels,
            int currentLevel = 1)
        {
            var friends = GenerateFriends(howMany, currentLevel);
            await _repository.InsertUsersAsFriends(root, friends);

            if (currentLevel < nestedLevels)
                foreach (var friend in friends)
                    await AddFriendRecursively(friend, Config.FriendsPerUser, nestedLevels, currentLevel + 1);
        }

        private static IEnumerable<Neo4JUserEntity> GenerateFriends(
            int howMany,
            int level)
            => Enumerable
                .Range(1, howMany)
                .Select(z => new Neo4JUserEntity
                {
                    Name = Config.UserName(level, z),
                    Id = Guid.NewGuid().ToString()
                }).ToList();


        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}