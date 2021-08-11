using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using Microsoft.EntityFrameworkCore;

namespace BenchmarkApp.Server.Database.SQL.Services
{
    public class SqlRepository : IDataLoader<SqlRepository>
    {
        private readonly SqlDatabaseContext _ctx;

        public SqlRepository(SqlDatabaseContext context) => _ctx = context;

        public async Task<int> LoadEntitiesAsync(int level)
        {
            var howMany = (int) Math.Pow(Config.FriendsPerUser, level + 1);
            var users = _ctx.Users.Take(howMany);
            var asyncUsers = await users.ToListAsync();
            return asyncUsers.Count;
        }

        public async Task<int> LoadAggregateAsync(int level)
        {
            var howMany = (int) Math.Pow(Config.FriendsPerUser, level + 1);
            var avg = _ctx.Users.Take(howMany).Average(t => t.Age);
            return howMany;
        }

        public async Task ConnectAsync() => await LoadEntitiesAsync(1);

        public async Task EmptyDatabaseAsync()
        {
            _ctx.Friendships.RemoveRange(_ctx.Friendships);
            _ctx.Users.RemoveRange(_ctx.Users);
            await _ctx.SaveChangesAsync();
        }

        public async Task<bool> IsDatabaseEmptyAsync() => await Task.FromResult(!_ctx.Users.Any());

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