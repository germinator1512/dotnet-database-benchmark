using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Mongo.Entities;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Services
{
    public class MongoRepository : IDataRepository<MongoRepository>
    {
        private readonly MongoDatabaseContext _ctx;
        private readonly FakeDataGeneratorService _faker;

        public MongoRepository(MongoDatabaseContext context, FakeDataGeneratorService faker)
        {
            _ctx = context;
            _faker = faker;
        }

        public async Task<int> LoadNestedEntitiesAsync(int level)
        {
            var user = await _ctx.Users
                .Find(u => u.Identifier == Config.RootUserName)
                .SingleAsync();

            await LoadFriendsRecursively(user, level);

            return (int) Math.Pow(Config.FriendsPerUser, level + 1);
        }

        public async Task<int> LoadEntitiesAsync(int level)
        {
            var howMany = (int) Math.Pow(Config.FriendsPerUser, level + 1);
            var users = await _ctx.Users.Find(_ => true).Limit(howMany).ToListAsync();
            return users.Count;
        }

        public async Task<int> LoadAggregateAsync(int level)
        {
            var howMany = (int) Math.Pow(Config.FriendsPerUser, level + 1);

            var agg = _ctx.Users.AsQueryable()
                .OrderBy(u => u.Id)
                .Take(howMany)
                .GroupBy(x => true)
                .Select(g => g.Average(s => s.Age))
                .Take(1);

            var average = agg.ToList()[0];
            return howMany;
        }

        public async Task<int> WriteEntitiesAsync(int level)
        {
            var howMany = (int) Math.Pow(Config.FriendsPerUser, level + 1);

            var friends = Enumerable
                .Range(1, howMany)
                .Select(z => _faker.GenerateFakeUser<MongoUserEntity>(Config.UserName(level, z)))
                .ToList();

            await _ctx.WriteUsers.InsertManyAsync(friends);

            return howMany;
        }

        public Task<int> WriteNestedEntitiesAsync(int level)
        {
            throw new NotImplementedException();
        }

        public async Task ConnectAsync() => await IsReadDatabaseEmptyAsync();

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

        public async Task EmptyWriteDatabaseAsync() => await _ctx.WriteUsers.DeleteManyAsync(u => true);
        public async Task<bool> IsReadDatabaseEmptyAsync() => !(await _ctx.Users.Find(_ => true).ToListAsync()).Any();

        public async Task EmptyReadDatabaseAsync() => await _ctx.Users.DeleteManyAsync(u => true);
    }
}