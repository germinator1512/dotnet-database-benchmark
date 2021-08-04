using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.SQL.Entities;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Database.SQL.Services
{
    public class PostgresInitializerService
    {
        private readonly SqlDatabaseContext _context;
        private readonly IDataLoader<SqlRepository> _repository;

        public PostgresInitializerService(SqlDatabaseContext context, IDataLoader<SqlRepository> repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task<InsertResult> InsertUserDataSet()
        {
            try
            {
                // await _context.Database.MigrateAsync();
                // await _context.SaveChangesAsync();
                //
                // await _repository.EmptyDatabase();
                //
                // if (_repository.IsDatabaseEmpty())
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
            Console.WriteLine("No entities found in PostgresDb - Inserting Test Dataset");

            var firstUser = new SqlUserEntity
            {
                Name = Config.RootUserName
            };

            await _context.Users.AddAsync(firstUser);

            await AddFriendRecursively(firstUser, Config.FriendsPerUser - 1, Config.NestedUserLevels);

            await _context.SaveChangesAsync();
        }


        /// <summary>
        /// adds nested user entities to database until given level is reached
        /// </summary>
        /// <param name="root">first user entity</param>
        /// <param name="howMany">number of new entities to be added to friends list of root user</param>
        /// <param name="nestedLevels">how many levels deep entities should be nested e.g 6 adds 10^6 users</param>
        /// <param name="currentLevel">current level of function call</param>
        private static async Task AddFriendRecursively(
            SqlUserEntity root,
            int howMany,
            int nestedLevels,
            int currentLevel = 1)
        {
            root.FriendShips = GenerateFriendsWithFriendShips(root, howMany, currentLevel);

            if (currentLevel < nestedLevels)
                foreach (var friend in root.FriendShips.Select(f => f.FriendB))
                    await AddFriendRecursively(friend, Config.FriendsPerUser, nestedLevels, currentLevel + 1);
        }


        private static List<SqlFriendshipEntity> GenerateFriendsWithFriendShips(
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
                    FriendB = new SqlUserEntity
                    {
                        Name = Config.UserName(level, z),
                    },
                }).ToList();
        }
    }
}