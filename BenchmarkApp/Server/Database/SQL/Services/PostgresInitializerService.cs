using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.SQL.Entities;
using BenchmarkApp.Shared;
using Microsoft.EntityFrameworkCore;

namespace BenchmarkApp.Server.Database.SQL.Services
{
    public class PostgresInitializerService
    {
        private readonly SqlDatabaseContext _context;
        private readonly IDataRepository<SqlRepository> _repository;
        private readonly FakeDataGeneratorService _faker;

        public PostgresInitializerService(SqlDatabaseContext context,
            IDataRepository<SqlRepository> repository,
            FakeDataGeneratorService faker)
        {
            _context = context;
            _repository = repository;
            _faker = faker;
        }

        public async Task<InsertResult> InsertUserDataSetAsync()
        {
            try
            {
                await _context.Database.MigrateAsync();
                await _context.SaveChangesAsync();

                await _repository.EmptyReadDatabaseAsync();

                if (await _repository.IsReadDatabaseEmptyAsync())
                    await AddDataSetAsync();

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
            Console.WriteLine("No entities found in PostgresDb - Inserting Test Dataset");

            var firstUser = _faker.GenerateFakeUser<SqlUserEntity>(Config.RootUserName);

            await _context.Users.AddAsync(firstUser);

            await AddFriendRecursiveAsync(firstUser, Config.FriendsPerUser - 1, Config.NestedUserLevels);

            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// adds nested user entities to database until given level is reached
        /// </summary>
        /// <param name="root">first user entity</param>
        /// <param name="howMany">number of new entities to be added to friends list of root user</param>
        /// <param name="nestedLevels">how many levels deep entities should be nested e.g 6 adds 10^6 users</param>
        /// <param name="currentLevel">current level of function call</param>
        private async Task AddFriendRecursiveAsync(
            SqlUserEntity root,
            int howMany,
            int nestedLevels,
            int currentLevel = 1)
        {
            root.FriendShips = GenerateFriendsWithFriendShips(root, howMany, currentLevel);

            if (currentLevel < nestedLevels)
                foreach (var friend in root.FriendShips.Select(f => f.FriendB))
                    await AddFriendRecursiveAsync(friend, Config.FriendsPerUser, nestedLevels, currentLevel + 1);
        }


        private List<SqlFriendshipEntity> GenerateFriendsWithFriendShips(
            SqlUserEntity rootFriend,
            int howMany,
            int level)
        {
            return Enumerable
                .Range(1, howMany)
                .Select(z => new SqlFriendshipEntity
                {
                    FriendA = rootFriend,
                    FriendAId = rootFriend.Id,
                    FriendB = _faker.GenerateFakeUser<SqlUserEntity>(Config.UserName(level, z))
                }).ToList();
        }
    }
}