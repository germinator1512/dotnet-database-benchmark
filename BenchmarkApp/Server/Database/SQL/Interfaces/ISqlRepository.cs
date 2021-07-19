using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.SQL.Entities;

namespace BenchmarkApp.Server.Database.SQL.Interfaces
{
    public interface ISqlRepository
    {
        Task<IEnumerable<SqlFriendshipEntity>> GetAllFriendsAsync(int level);

        Task EmptyDatabase(CancellationToken cancellationToken);
        Task<bool> IsDatabaseEmpty(CancellationToken cancellationToken);
    }
}