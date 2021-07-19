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
                .Find(u => u.Name == Config.RootUserName)
                .SingleAsync();

            await LoadFriendsRecursively(user, level);

            return user.FriendShips;
        }

        public async Task<bool> IsDatabaseEmpty(CancellationToken cancellationToken)
            => (await _ctx.Users.Find(_ => true).FirstAsync(cancellationToken)) == null;

        private async Task<MongoUserEntity> LoadFriendsRecursively(
            MongoUserEntity root,
            int nestedLevels,
            int currentLevel = 1)
        {
            await root.LoadFriendShips(_ctx);
            foreach (var friendship in root.FriendShips)
            {
                await friendship.LoadFriend(_ctx);
                if (currentLevel < nestedLevels)
                {
                    return await LoadFriendsRecursively(friendship.FriendB, nestedLevels, currentLevel + 1);
                }
            }

            return root;
        }
    }
}