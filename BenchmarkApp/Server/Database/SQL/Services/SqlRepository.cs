using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.SQL.Entities;
using BenchmarkApp.Server.Database.SQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BenchmarkApp.Server.Database.SQL.Services
{
    public class SqlRepository : ISqlRepository
    {
        private readonly SqlDatabaseContext _ctx;

        public SqlRepository(SqlDatabaseContext context) => _ctx = context;

        public Task<List<SqlUserEntity>> GetUserAsync(int howMany)
        {
            var users = _ctx.Users.Take(howMany);
            return users.ToListAsync();
        }

        // user to avoid "cold start" of database in first benchmark run
        public async Task ConnectAsync()
        {
            await GetUserAsync((int) Math.Pow(5, 5));
        }

        public async Task EmptyDatabase()
        {
            _ctx.Friendships.RemoveRange(_ctx.Friendships);
            _ctx.Users.RemoveRange(_ctx.Users);
            await _ctx.SaveChangesAsync();
        }

        public bool IsDatabaseEmpty() => !_ctx.Users.Any();

        public async Task<IEnumerable<SqlFriendshipEntity>> GetAllFriendsAsync(int level)
        {
            var firstUser = await _ctx.Users.SingleAsync(u => u.Name.Equals(Config.RootUserName));

            return await _ctx.Friendships
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
        }
    }
}