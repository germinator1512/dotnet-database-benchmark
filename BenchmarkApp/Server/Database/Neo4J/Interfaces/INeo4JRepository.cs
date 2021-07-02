using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;

namespace BenchmarkApp.Server.Database.Neo4J.Interfaces
{
    public interface INeo4JRepository
    {
        Task<IEnumerable<IUserEntity>> GetAllEntitiesAsync();
    }
}