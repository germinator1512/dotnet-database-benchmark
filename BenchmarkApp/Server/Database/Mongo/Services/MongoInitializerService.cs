using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
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


        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private static async Task AddDataSet(MongoDatabaseContext context)
        {
            Console.WriteLine("No entities found in MongoDB - Inserting Test Dataset");

            var users = context.Users;
            var friendships = context.FriendShips;

            var firstUser = new MongoUserEntity
            {
                Name = EntityConfig.RootUserName,
            };

            await users.InsertOneAsync(firstUser);


            var level1Friends = GenerateFriends(9, 1);
            await users.InsertManyAsync(level1Friends);

            var friendShipList = GenerateFriendShips(firstUser, level1Friends);
            await friendships.InsertManyAsync(friendShipList);


            foreach (var level1Friend in level1Friends)
            {
                var level2Friends = GenerateFriends(10, 2);
                await users.InsertManyAsync(level2Friends);


                var level1FriendShips = GenerateFriendShips(level1Friend, level2Friends);
                await friendships.InsertManyAsync(level1FriendShips);

                foreach (var level2Friend in level2Friends)
                {
                    var level3Friends = GenerateFriends(10, 3);
                    await users.InsertManyAsync(level3Friends);

                    var level2FriendShips = GenerateFriendShips(level2Friend, level3Friends);
                    await friendships.InsertManyAsync(level2FriendShips);

                    foreach (var level3Friend in level3Friends)
                    {
                        var level4Friends = GenerateFriends(10, 4);
                        await users.InsertManyAsync(level4Friends);

                        var level3FriendShips = GenerateFriendShips(level3Friend, level4Friends);
                        await friendships.InsertManyAsync(level3FriendShips);

                        foreach (var level4Friend in level4Friends)
                        {
                            var level5Friends = GenerateFriends(10, 5);
                            await users.InsertManyAsync(level5Friends);

                            var level4FriendShips = GenerateFriendShips(level4Friend, level5Friends);
                            await friendships.InsertManyAsync(level4FriendShips);

                            foreach (var level5Friend in level5Friends)
                            {
                                var level6Friends = GenerateFriends(10, 6);
                                await users.InsertManyAsync(level6Friends);

                                var level5FriendShips = GenerateFriendShips(level5Friend, level6Friends);
                                await friendships.InsertManyAsync(level5FriendShips);
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<MongoUserEntity> GenerateFriends(
            int howMany,
            int level)
            => Enumerable
                .Range(1, howMany)
                .Select(z => new MongoUserEntity
                {
                    Name = EntityConfig.UserName(level, z),
                });

        private static IEnumerable<MongoFriendShipEntity> GenerateFriendShips(
            MongoUserEntity rootFriend,
            IEnumerable<MongoUserEntity> friends)
            => friends.Select(f => new MongoFriendShipEntity
            {
                FriendARef = new MongoDBRef("users", rootFriend.Id),
                FriendBRef = new MongoDBRef("users", f.Id),
            });
    }
}