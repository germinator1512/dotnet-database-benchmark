using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Neo4J.Services;
using BenchmarkApp.Server.Services.Interfaces;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Services
{
    public class Neo4JBenchmarkService : IBenchmarkService<Neo4JBenchmarkService>
    {
        private readonly IDataLoader<Neo4JRepository> _neo4JRepository;

        public Neo4JBenchmarkService(IDataLoader<Neo4JRepository> neo4JRepository) =>
            _neo4JRepository = neo4JRepository;

        public async Task<IEnumerable<BenchmarkResult>> StartFriendsWithNeighboursBenchmarkAsync() =>
            await TimerService.Benchmark(_neo4JRepository, _neo4JRepository.LoadNestedEntities);

        public async Task<IEnumerable<BenchmarkResult>> StartUserBenchmarkAsync() =>
            await TimerService.Benchmark(_neo4JRepository, _neo4JRepository.LoadEntities);
    }
}