using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.SQL.Entities;

namespace BenchmarkApp.Server.Database.SQL.Interfaces
{
    public interface ISqlRepository
    {
        Task<IEnumerable<SqlFriendshipEntity>> GetAllFriendsAsync(int level);

        Task<List<SqlUserEntity>> GetUserAsync(int howMany);
        
        Task ConnectAsync();

        Task EmptyDatabase();

        bool IsDatabaseEmpty();
    }
}