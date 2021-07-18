using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Neo4J.Interfaces;
using BenchmarkApp.Server.Services.Interfaces;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Services
{
    public class Neo4JBenchmarkService : INeo4JBenchmarkService
    {
        private readonly INeo4JRepository _neo4JRepository;
        public Neo4JBenchmarkService(INeo4JRepository neo4JRepository) => _neo4JRepository = neo4JRepository;

        public async Task<BenchmarkResult> StartBenchmark()
        {
            var entities = await _neo4JRepository.GetAllFriendsAsync(3);
            return new BenchmarkResult
            {
                Success = true
            };
        }
    }
}