﻿using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<MongoUserEntity>> GetAllFriendsAsync(int level)
        {
            var user = await _ctx.Users
                .Find(u => u.Name == Config.RootUserName)
                .SingleAsync();

            await LoadFriendsRecursively(user, level);

            return user.Friends;
        }

        public async Task<IEnumerable<MongoUserEntity>> GetUserAsync(int howMany)
        {
            return await _ctx.Users.Find(_ => true).Limit(howMany).ToListAsync();
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