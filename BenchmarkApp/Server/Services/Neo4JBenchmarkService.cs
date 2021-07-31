using System.Diagnostics;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
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
            var timer = new Stopwatch();
            timer.Start();

            var entities = await _neo4JRepository.GetAllFriendsAsync(Config.Level);

            timer.Stop();
            return new BenchmarkResult
            {
                Level = Config.Level,
                Success = true,
                TimeSpan = timer.Elapsed
            };
        }
    }
}