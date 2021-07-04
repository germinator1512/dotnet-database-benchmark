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
                .ThenInclude(f => f.FriendShips)
                
                .Include(f => f.FriendB)
                .ThenInclude(f => f.FriendShips)
                
                .Include(f => f.FriendB)
                .ThenInclude(f => f.FriendShips)
                
                .Include(f => f.FriendB)
                .ThenInclude(f => f.FriendShips)
                
                .Include(f => f.FriendB)
                .ThenInclude(f => f.FriendShips)
                
                .Include(f => f.FriendB)
                .ThenInclude(f => f.FriendShips)
                
                .ToListAsync();
        }


        public async Task AddEntitiesAsync(IEnumerable<SqlUserEntity> users)
        {
            await _ctx.Users.AddRangeAsync(users);
            await _ctx.SaveChangesAsync();
        }
    }
}