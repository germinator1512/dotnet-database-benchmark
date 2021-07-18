using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Neo4J.Entities;

namespace BenchmarkApp.Server.Database.Neo4J.Interfaces
{
    public interface INeo4JRepository
    {
        Task<IEnumerable<Neo4jUserEntity>> GetAllFriendsAsync(int level);
    }
}