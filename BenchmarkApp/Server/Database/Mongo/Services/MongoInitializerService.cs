using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Mongo.Entities;
using BenchmarkApp.Shared;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Services
{
    public class MongoInitializerService
    {
        private readonly IDataLoader<MongoRepository> _mongoRepository;
        private readonly MongoDatabaseContext _context;
        private readonly FakeDataGeneratorService _faker;

        public MongoInitializerService(IDataLoader<MongoRepository> repository,
            MongoDatabaseContext context,
            FakeDataGeneratorService faker)
        {
            _mongoRepository = repository;
            _context = context;
            _faker = faker;
        }

        public async Task<InsertResult> InsertUserDataSetAsync()
        {
            try
            {
                // await _mongoRepository.EmptyDatabaseAsync();

                // if (await _mongoRepository.IsDatabaseEmptyAsync())
                // await AddDataSet();

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
            Console.WriteLine("No entities found in MongoDB - Inserting Test Dataset");

            var firstUser = _faker.GenerateFakeUser<MongoUserEntity>(Config.RootUserName);

            await AddFriendRecursiveAsync(firstUser, Config.FriendsPerUser - 1, Config.NestedUserLevels);
            await _context.Users.InsertOneAsync(firstUser);
        }

        /// <summary>
        /// adds nested user entities to database until given level is reached
        /// </summary>
        /// <param name="root">first user entity</param>
        /// <param name="howMany">number of new entities to be added to friends list of root user</param>
        /// <param name="nestedLevels">how many levels deep entities should be nested e.g 6 adds 10^6 users</param>
        /// <param name="currentLevel">current level of function call</param>
        private async Task AddFriendRecursiveAsync(
            MongoUserEntity root,
            int howMany,
            int nestedLevels,
            int currentLevel = 1)
        {
            List<MongoUserEntity> GenerateFriends() => Enumerable
                .Range(1, howMany)
                .Select(z => _faker.GenerateFakeUser<MongoUserEntity>(Config.UserName(currentLevel, z)))
                .ToList();

            var friends = GenerateFriends();

            if (currentLevel < nestedLevels)
                foreach (var friend in friends)
                    await AddFriendRecursiveAsync(friend, Config.FriendsPerUser, nestedLevels, currentLevel + 1);

            await _context.Users.InsertManyAsync(friends);

            friends.ForEach(f => root.FriendIds.Add(new MongoDBRef("users", f.Id)));
        }
    }
}