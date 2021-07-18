using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Neo4J.Entities;
using BenchmarkApp.Server.Database.Neo4J.Interfaces;
using Neo4jClient;

namespace BenchmarkApp.Server.Database.Neo4J.Services
{
    public class Neo4JRepository : INeo4JRepository
    {
        private readonly IGraphClient _ctx;

        public Neo4JRepository(IGraphClient client) => _ctx = client;

        public async Task<IEnumerable<Neo4JUserEntity>> GetAllFriendsAsync(int level)
        {
            var result = await _ctx.Cypher
                .Match(@"(user:User{name: $name})-[:KNOWS]->(friend:User)")
                .WithParam("name", "Max Mustermann")
                .Return<Neo4JUserEntity>("friend")
                .ResultsAsync;


            return new List<Neo4JUserEntity>();
        }
    }
}