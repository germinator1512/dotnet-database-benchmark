using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.SQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BenchmarkApp.Server.Database.SQL.Services
{
    public class PostgresInitializerService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public PostgresInitializerService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SqlDatabaseContext>();
            await context.Database.MigrateAsync(cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            await EmptyDatabase(cancellationToken, context);

            if (!context.Users.Any()) await AddDataSet(context);
        }

        private static async Task EmptyDatabase(CancellationToken cancellationToken, SqlDatabaseContext context)
        {
            context.Friendships.RemoveRange(context.Friendships);
            context.Users.RemoveRange(context.Users);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// adds one million user entities to database which are nested 6 levels deep
        /// </summary>
        /// <param name="context"></param>
        private async Task AddDataSet(SqlDatabaseContext context)
        {
            Console.WriteLine("No entities found in PostgresDb - Inserting Test Dataset");

            var firstUser = new SqlUserEntity
            {
                Name = "Max Mustermann"
            };

            await context.Users.AddAsync(firstUser);
            await AddFriendRecursively(context, firstUser, 10, 3, 0);

            await context.SaveChangesAsync();
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
            SqlDatabaseContext context,
            SqlUserEntity root,
            int howMany,
            int level,
            int currentDepth)
        {
            var friends = GenerateFriends(root, howMany, currentDepth);
            await context.Users.AddRangeAsync(friends);

            if (level >= currentDepth)
            {
                foreach (var friend in friends)
                {
                    await AddFriendRecursively(context, friend, 10, level, ++currentDepth);
                }
            }
        }

        private static List<SqlUserEntity> GenerateFriends(
            SqlUserEntity rootFriend,
            int howMany,
            int level)
        {
            var newFriends = new List<SqlUserEntity>();
            for (var z = 1; z <= howMany; z++)
            {
                var friend = new SqlUserEntity
                {
                    Name = $"Level {level} Friend {z}"
                };
                rootFriend.FriendShips.Add(new SqlFriendshipEntity
                {
                    FriendA = rootFriend,
                    FriendB = friend
                });
                newFriends.Add(friend);
            }

            return newFriends;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}