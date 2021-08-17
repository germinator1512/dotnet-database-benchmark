using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.SQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BenchmarkApp.Server.Database.SQL.Services
{
    public class SqlRepository : IDataRepository<SqlRepository>
    {
        private readonly SqlDatabaseContext _ctx;
        private readonly FakeDataGeneratorService _faker;

        public SqlRepository(SqlDatabaseContext context, FakeDataGeneratorService faker)
        {
            _ctx = context;
            _faker = faker;
        }

        public async Task<int> LoadEntitiesAsync(int level)
        {
            var howMany = (int) Math.Pow(Config.FriendsPerUser, level + 1);

            var users = await _ctx.Users
                .OrderBy(u => u.Id)
                .Take(howMany)
                .ToListAsync();

            return (users).Count;
        }

        public async Task<int> LoadAggregateAsync(int level)
        {
            var howMany = (int) Math.Pow(Config.FriendsPerUser, level + 1);

            var avg = _ctx.Users
                .OrderBy(u => u.Id)
                .Take(howMany)
                .Average(t => t.Age);

            return howMany;
        }

        public async Task<int> WriteEntitiesAsync(int level)
        {
            var howMany = (int) Math.Pow(Config.FriendsPerUser, level + 1);

            var names = Enumerable.Range(1, howMany).Select(i => Config.UserName(level, i)).ToList();
            var fakeUsers = _faker.GenerateFakeUsers<SqlWriteUserEntity>(names);

            await _ctx.WriteUsers.AddRangeAsync(fakeUsers);
            await _ctx.SaveChangesAsync();
            return howMany;
        }


        public async Task<int> WriteNestedEntitiesAsync(int level)
        {
            var howMany = (int) Math.Pow(Config.FriendsPerUser, level + 1);


            var firstUser = _faker.GenerateFakeUser<SqlWriteUserEntity>(Config.RootUserName);
            await _ctx.WriteUsers.AddAsync(firstUser);


            var friends = Enumerable
                .Range(1, howMany)
                .Select(z => new SqlWriteFriendshipEntity()
                {
                    FriendA = firstUser,
                    FriendAId = firstUser.Id,
                    FriendB = _faker.GenerateFakeUser<SqlWriteUserEntity>(Config.UserName(level, z))
                }).ToList();

            await _ctx.WriteFriendships.AddRangeAsync(friends);

            await _ctx.SaveChangesAsync();

            return howMany;
        }


        public async Task ConnectAsync() => await LoadEntitiesAsync(1);

        public async Task EmptyReadDatabaseAsync()
        {
            _ctx.Friendships.RemoveRange(_ctx.Friendships);
            _ctx.Users.RemoveRange(_ctx.Users);
            await _ctx.SaveChangesAsync();
        }

        public async Task EmptyWriteDatabaseAsync()
        {
            _ctx.WriteFriendships.RemoveRange(_ctx.WriteFriendships);
            _ctx.WriteUsers.RemoveRange(_ctx.WriteUsers);
            await _ctx.SaveChangesAsync();
        }

        public async Task<bool> IsReadDatabaseEmptyAsync() => await Task.FromResult(!_ctx.Users.Any());

        public async Task<int> LoadNestedEntitiesAsync(int level)
        {
            var firstUser = await _ctx.Users.SingleAsync(u => u.Identifier.Equals(Config.RootUserName));

            var all = await _ctx.Friendships
                .Where(f => f.FriendA.Id == firstUser.Id)
                .Include(f => f.FriendB)
                .If(level > 0, level1 => level1
                    .ThenInclude(user => user.FriendShips)
                    .ThenInclude(f => f.FriendB)
                    .If(level > 1, level2 => level2
                        .ThenInclude(user => user.FriendShips)
                        .ThenInclude(f => f.FriendB)
                        .If(level > 2, level3 => level3
                            .ThenInclude(user => user.FriendShips)
                            .ThenInclude(f => f.FriendB)
                            .If(level > 3, level4 => level4
                                .ThenInclude(user => user.FriendShips)
                                .ThenInclude(f => f.FriendB)
                                .If(level > 4, level5 => level5
                                    .ThenInclude(user => user.FriendShips)
                                    .ThenInclude(f => f.FriendB)
                                )))))
                .ToListAsync();

            return (int) Math.Pow(Config.FriendsPerUser, level + 1);
        }
    }
}