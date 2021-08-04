using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Mongo.Entities;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Services
{
    public class MongoRepository : IDataLoader<MongoRepository>
    {
        private readonly MongoDatabaseContext _ctx;
        public MongoRepository(MongoDatabaseContext context) => _ctx = context;

        public async Task<int> GetAllFriendsAsync(int level)
        {
            var user = await _ctx.Users
                .Find(u => u.Name == Config.RootUserName)
                .SingleAsync();

            await LoadFriendsRecursively(user, level);

            return (int) Math.Pow(Config.FriendsPerUser, level + 1);
        }

        public async Task<int> GetUserAsync(int level)
        {
            var howMany = (int) Math.Pow(Config.FriendsPerUser, level + 1);
            var users = await _ctx.Users.Find(_ => true).Limit(howMany).ToListAsync();
            return users.Count;
        }

        public async Task ConnectAsync() => await IsDatabaseEmpty();

        private async Task LoadFriendsRecursively(
            MongoUserEntity root,
            int nestedLevels,
            int currentLevel = 0)
        {
            await root.LoadFriends(_ctx);

            if (currentLevel < nestedLevels)
                foreach (var friend in root.Friends)
                    await LoadFriendsRecursively(friend, nestedLevels, currentLevel + 1);
        }

        public async Task<bool> IsDatabaseEmpty() => !(await _ctx.Users.Find(_ => true).ToListAsync()).Any();

        public async Task EmptyDatabase() => await _ctx.Users.DeleteManyAsync(u => true);
    }
}