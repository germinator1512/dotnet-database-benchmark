using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Mongo.Entities;
using BenchmarkApp.Server.Database.Mongo.Interfaces;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Services
{
    public class MongoRepository : IMongoRepository
    {
        private readonly MongoDatabaseContext _ctx;

        public MongoRepository(MongoDatabaseContext context) => _ctx = context;

        public async Task<IEnumerable<MongoFriendShipEntity>> GetAllFriendsAsync(int level)
        {
            var user = await _ctx.Users
                .Find(u => u.Name == EntityConfig.RootUserName)
                .SingleAsync();

            await user.LoadFriendShips(_ctx);
            foreach (var friendShip in user.FriendShips)
            {
                await friendShip.LoadFriend(_ctx);


                if (level > 0)
                {
                    await friendShip.FriendB.LoadFriendShips(_ctx);
                    foreach (var friendship1 in friendShip.FriendB.FriendShips)
                    {
                        await friendship1.LoadFriend(_ctx);

                        if (level > 1)
                        {
                            await friendship1.FriendB.LoadFriendShips(_ctx);
                            foreach (var friendship2 in friendShip.FriendB.FriendShips)
                            {
                                await friendship2.LoadFriend(_ctx);

                                if (level > 2)
                                {
                                    await friendship2.FriendB.LoadFriendShips(_ctx);

                                    foreach (var friendship3 in friendShip.FriendB.FriendShips)
                                    {
                                        await friendship3.LoadFriend(_ctx);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return user.FriendShips;
        }

        public async Task<bool> IsDatabaseEmpty(CancellationToken cancellationToken)
            => (await _ctx.Users.Find(_ => true).FirstAsync(cancellationToken)) == null;

        // private async Task<MongoFriendShipEntity> LoadFriendsRecursively(
        //     MongoFriendShipEntity root,
        //     int level,
        //     int currentDepth)
        // {
        //     await root.LoadFriend(_ctx);
        //
        //     if (level > currentDepth)
        //     {
        //         await root.FriendB.LoadFriendShips(_ctx);
        //         foreach (var friendship in root.FriendB.FriendShips)
        //         {
        //             return await LoadFriendsRecursively(friendship, level, ++currentDepth);
        //         }
        //     }
        //
        //     return root;
        // }
    }
}