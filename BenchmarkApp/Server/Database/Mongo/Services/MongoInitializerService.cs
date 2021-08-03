using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Mongo.Entities;
using BenchmarkApp.Server.Database.Mongo.Interfaces;
using BenchmarkApp.Shared;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Services
{
    public class MongoInitializerService
    {
        private readonly IMongoRepository _repository;
        private readonly MongoDatabaseContext _context;

        public MongoInitializerService(IMongoRepository repository, MongoDatabaseContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<InsertResult> InsertUserDataSet()
        {
            try
            {
                // await repository.EmptyDatabase(cancellationToken);

                // if (await _repository.IsDatabaseEmpty())
                //     await AddDataSet();

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


        private async Task AddDataSet()
        {
            Console.WriteLine("No entities found in MongoDB - Inserting Test Dataset");

            var firstUser = new MongoUserEntity
            {
                Name = Config.RootUserName,
            };

            await AddFriendRecursively(firstUser, Config.FriendsPerUser - 1, Config.NestedUserLevels);
            await _context.Users.InsertOneAsync(firstUser);
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

            if (currentLevel < nestedLevels)
                foreach (var friend in friends)
                    await AddFriendRecursively(friend, Config.FriendsPerUser, nestedLevels, currentLevel + 1);

            await _context.Users.InsertManyAsync(friends);

            friends.ForEach(f => root.FriendIds.Add(new MongoDBRef("users", f.Id)));
        }
    }
}