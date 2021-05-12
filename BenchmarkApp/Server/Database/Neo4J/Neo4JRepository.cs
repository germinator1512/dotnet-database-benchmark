using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Entities;

namespace BenchmarkApp.Server.Database.Neo4J
{
    public class Neo4JRepository : INeo4JRepository
    {
        private readonly Neo4JDatabaseContext _ctx;

        public Neo4JRepository(Neo4JDatabaseContext context) => _ctx = context;

        public Task<IEnumerable<Entity>> GetAllEntities()
        {
            throw new System.NotImplementedException();
        }
    }
}