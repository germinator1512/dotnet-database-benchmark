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

            // await EmptyDatabase(cancellationToken, context);
            //
            // if (!context.Users.Any()) await AddDataSet(context);
        }

        private async Task EmptyDatabase(CancellationToken cancellationToken, SqlDatabaseContext context)
        {
            context.Friendships.RemoveRange(context.Friendships);
            context.Users.RemoveRange(context.Users);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// adds one million user entities to database which are nested 6 levels deep
        /// </summary>
        /// <param name="context"></param>
        private static async Task AddDataSet(SqlDatabaseContext context)
        {
            Console.WriteLine("No entities found in PostgresDb - Inserting Test Dataset");

            var firstUser = new SqlUserEntity
            {
                Name = "Max Mustermann"
            };

            await context.Users.AddAsync(firstUser);

            var level1Friends = GenerateFriends(firstUser, 9, 1);
            await context.Users.AddRangeAsync(level1Friends);

            foreach (var level1Friend in level1Friends)
            {
                var level2Friends = GenerateFriends(level1Friend, 10, 2);
                await context.Users.AddRangeAsync(level2Friends);

                foreach (var level2Friend in level2Friends)
                {
                    var level3Friends = GenerateFriends(level2Friend, 10, 3);
                    await context.Users.AddRangeAsync(level3Friends);

                    foreach (var level3Friend in level3Friends)
                    {
                        var level4Friends = GenerateFriends(level3Friend, 10, 4);
                        await context.Users.AddRangeAsync(level4Friends);

                        foreach (var level4Friend in level4Friends)
                        {
                            var level5Friends = GenerateFriends(level4Friend, 10, 5);
                            await context.Users.AddRangeAsync(level5Friends);

                            foreach (var level5Friend in level5Friends)
                            {
                                var level6Friends = GenerateFriends(level5Friend, 10, 6);
                                await context.Users.AddRangeAsync(level6Friends);
                            }
                        }
                    }
                }
            }

            await context.SaveChangesAsync();
        }

        private static List<SqlUserEntity> GenerateFriends(SqlUserEntity rootFriend,
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