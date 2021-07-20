using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public async Task EmptyDatabase(CancellationToken cancellationToken)
        {
            _ctx.Friendships.RemoveRange(_ctx.Friendships);
            _ctx.Users.RemoveRange(_ctx.Users);
            await _ctx.SaveChangesAsync(cancellationToken);
        }

        public bool IsDatabaseEmpty(CancellationToken cancellationToken) => !_ctx.Users.Any();

        public async Task<IEnumerable<SqlFriendshipEntity>> GetAllFriendsAsync(int level)
        {
            var firstUser = await _ctx.Users.SingleAsync(u => u.Name.Equals(Config.RootUserName));

            var query =
                _ctx.Friendships
                    .Where(f => f.FriendA.Id == firstUser.Id)
                    .Include(f => f.FriendB);

            foreach (var _ in Enumerable.Range(0, level))
            {
                query
                    .ThenInclude(user => user.FriendShips)
                    .ThenInclude(f => f.FriendB);
            }

            return await query.ToListAsync();
        }
    }
}