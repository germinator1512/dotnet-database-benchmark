using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Neo4J.Entities;

namespace BenchmarkApp.Server.Database.Neo4J.Interfaces
{
    public interface INeo4JRepository
    {
        Task ConnectAsync();
        Task<IEnumerable<Neo4JUserEntity>> GetAllFriendsAsync(int level);

        Task<IEnumerable<Neo4JUserEntity>> GetUserAsync(int howMany);

        Task EmptyDatabase();

        Task<bool> IsDatabaseEmpty();

        Task InsertSingleUser(Neo4JUserEntity single);

        Task InsertUsersAsFriends(Neo4JUserEntity rootUser, IEnumerable<Neo4JUserEntity> friends);
    }
}