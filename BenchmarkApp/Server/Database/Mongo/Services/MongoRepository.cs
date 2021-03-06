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

            var average = _ctx.Users.AsQueryable()
                .OrderBy(u => u.Id)
                .Take(howMany)
                .GroupBy(x => true)
                .Select(g => g.Average(s => s.Age))
                .First();
            
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

        public async Task<int> WriteNestedEntitiesAsync(int level)
        {
            var howMany = (int) Math.Pow(Config.FriendsPerUser, level + 1);

            var firstUser = _faker.GenerateFakeUser<MongoUserEntity>(Config.RootUserName);


            var friends = Enumerable
                .Range(1, howMany)
                .Select(z => _faker.GenerateFakeUser<MongoUserEntity>(Config.UserName(level, z)))
                .ToList();

            await _ctx.WriteUsers.InsertManyAsync(friends);

            firstUser.FriendIds.AddRange(friends.Select(f => new MongoDBRef("users", f.Id)));

            await _ctx.WriteUsers.InsertOneAsync(firstUser);

            return howMany;
        }

        public async Task ConnectAsync() => await  LoadEntitiesAsync(1);

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