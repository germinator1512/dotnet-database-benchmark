using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Neo4J.Entities;
using BenchmarkApp.Shared;
using Neo4jClient;

namespace BenchmarkApp.Server.Database.Neo4J.Services
{
    public class Neo4JInitializerService
    {
        private readonly IDataLoader<Neo4JRepository> _repository;
        private readonly IGraphClient _client;
        private readonly FakeDataGeneratorService _faker;

        public Neo4JInitializerService(IDataLoader<Neo4JRepository> neo4JRepository,
            IGraphClient client,
            FakeDataGeneratorService faker)
        {
            _repository = neo4JRepository;
            _client = client;
            _faker = faker;
        }

        public async Task<InsertResult> InsertUserDataSetAsync()
        {
            try
            {
                await _client.ConnectAsync();
                // await _repository.EmptyDatabaseAsync();

                // var isEmpty = await _repository.IsDatabaseEmptyAsync();
                // if (isEmpty) await AddDataSet();

                return new InsertResult
                {
                    Success = true
                };
            }
            catch (Exception e)
            {
                return new InsertResult
                {
                    Success = false,
                    ErrorMessage = e.Message
                };
            }
        }

        private async Task AddDataSetAsync()
        {
            Console.WriteLine("No entities found in Neo4J - Inserting Test Dataset");

            const string firstName = Config.RootUserName;

            var firstUser = _faker.GenerateFakeUser<Neo4JUserEntity>(Config.RootUserName);
            firstUser.Id = Guid.NewGuid().ToString();

            await InsertSingleUserAsync(firstUser);

            await AddFriendRecursiveAsync(firstUser, Config.FriendsPerUser - 1, Config.NestedUserLevels);
        }

        /// <summary>
        /// adds nested user entities to database until given level is reached
        /// </summary>
        /// <param name="root">first user entity</param>
        /// <param name="howMany">number of new entities to be added to friends list of root user</param>
        /// <param name="nestedLevels">how many levels deep entities should be nested e.g 6 adds 10^6 users</param>
        /// <param name="currentLevel">current level of function call</param>
        private async Task AddFriendRecursiveAsync(
            Neo4JUserEntity root,
            int howMany,
            int nestedLevels,
            int currentLevel = 1)
        {
            var friends = GenerateFriends(howMany, currentLevel);
            await InsertUsersAsFriendsAsync(root, friends);

            if (currentLevel < nestedLevels)
                foreach (var friend in friends)
                    await AddFriendRecursiveAsync(friend, Config.FriendsPerUser, nestedLevels, currentLevel + 1);
        }

        private IEnumerable<Neo4JUserEntity> GenerateFriends(
            int howMany,
            int level)
            => Enumerable
                .Range(1, howMany)
                .Select(z =>
                {
                    var user = _faker.GenerateFakeUser<Neo4JUserEntity>(Config.UserName(level, z));
                    user.Id = Guid.NewGuid().ToString();
                    return user;
                }).ToList();

        private async Task InsertSingleUserAsync(Neo4JUserEntity single)
            => await _client.Cypher
                .Create("(user:User {name: $name, id: $id})")
                .WithParams(single.ToMap())
                .ExecuteWithoutResultsAsync();

        private async Task InsertUsersAsFriendsAsync(Neo4JUserEntity rootUser, IEnumerable<Neo4JUserEntity> friends)
            => await _client.Cypher
                .Match("(root:User)")
                .Where((Neo4JUserEntity root) => root.Id == rootUser.Id)
                .Unwind(friends, "friend")
                .Merge("(user:User {name: friend.name, id: friend.id}) <-[:KNOWS]-(root)")
                .WithParam("rootId", rootUser.Id)
                .ExecuteWithoutResultsAsync();
    }
}