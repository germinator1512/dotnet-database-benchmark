using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.SQL.Entities;
using BenchmarkApp.Server.Database.SQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BenchmarkApp.Server.Database.SQL.Services
{
    public class SqlRepository : ISqlRepository
    {
        private readonly SqlDatabaseContext _ctx;

        public SqlRepository(SqlDatabaseContext context) => _ctx = context;

        public async Task<IEnumerable<SqlFriendshipEntity>> GetAllFriendsAsync(int depth)
        {
            var firstUser = await _ctx.Users.SingleAsync(u => u.Name.Equals("Max Mustermann"));

            return await _ctx.Friendships
                .Where(f => f.FriendA.Id == firstUser.Id)
                .Include(f => f.FriendB)
                .If(depth > 0, level1 => level1
                    .ThenInclude(user => user.FriendShips)
                    .ThenInclude(f => f.FriendB)
                    .If(depth > 1, level2 => level2
                        .ThenInclude(user => user.FriendShips)
                        .ThenInclude(f => f.FriendB)
                        .If(depth > 2, level3 => level3
                            .ThenInclude(user => user.FriendShips)
                            .ThenInclude(f => f.FriendB)
                            .If(depth > 3, level4 => level4
                                .ThenInclude(user => user.FriendShips)
                                .ThenInclude(f => f.FriendB)
                                .If(depth > 4, level5 => level5
                                    .ThenInclude(user => user.FriendShips)
                                    .ThenInclude(f => f.FriendB)
                                    .If(depth > 5, level6 => level6
                                        .ThenInclude(user => user.FriendShips)
                                        .ThenInclude(f => f.FriendB)))))))
                .ToListAsync();
        }


        public async Task AddEntitiesAsync(IEnumerable<SqlUserEntity> users)
        {
            await _ctx.Users.AddRangeAsync(users);
            await _ctx.SaveChangesAsync();
        }
    }
}