using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Services.Interfaces;
using BenchmarkApp.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BenchmarkApp.Server.Controllers
{
    [ApiController]
    [Route("benchmark")]
    public class BenchmarkController : ControllerBase
    {
        private readonly ILogger<BenchmarkController> _logger;
        private readonly IMongoBenchmarkService _mongoBenchmarkService;
        private readonly ISQLBenchmarkService _sqlBenchmarkService;
        private readonly INeo4JBenchmarkService _neo4JBenchmarkService;

        public BenchmarkController(ILogger<BenchmarkController> logger,
            IMongoBenchmarkService mongoBenchmarkService,
            ISQLBenchmarkService sqlBenchmarkService,
            INeo4JBenchmarkService neo4JBenchmarkService
        )
        {
            _logger = logger;
            _mongoBenchmarkService = mongoBenchmarkService;
            _sqlBenchmarkService = sqlBenchmarkService;
            _neo4JBenchmarkService = neo4JBenchmarkService;
        }

        [HttpGet("mongo/neighbour")]
        public async Task<IEnumerable<BenchmarkResult>> MongoNeighbour()
        {
            _logger.Log(LogLevel.Debug, "Starting Mongo Benchmark");
            return await _mongoBenchmarkService.StartFriendsWithNeighboursBenchmarkAsync();
        }

        [HttpGet("mongo/user")]
        public async Task<IEnumerable<BenchmarkResult>> MongoUser()
        {
            _logger.Log(LogLevel.Debug, "Starting Mongo Benchmark");
            return await _mongoBenchmarkService.StartUserBenchmarkAsync();
        }

        [HttpGet("sql/neighbour")]
        public async Task<IEnumerable<BenchmarkResult>> SqlNeighbour()
        {
            _logger.Log(LogLevel.Debug, "Starting SQL Benchmark");
            return await _sqlBenchmarkService.StartFriendsWithNeighboursBenchmarkAsync();
        }

        [HttpGet("sql/user")]
        public async Task<IEnumerable<BenchmarkResult>> SqlUser()
        {
            _logger.Log(LogLevel.Debug, "Starting SQL Benchmark");
            return await _sqlBenchmarkService.StartUserBenchmarkAsync();
        }

        [HttpGet("neo4j/neighbour")]
        public async Task<IEnumerable<BenchmarkResult>> Neo4JNeighbour()
        {
            _logger.Log(LogLevel.Debug, "Starting Neo4J Benchmark");
            return await _neo4JBenchmarkService.StartFriendsWithNeighboursBenchmarkAsync();
        }

        [HttpGet("neo4j/user")]
        public async Task<IEnumerable<BenchmarkResult>> Neo4JUser()
        {
            _logger.Log(LogLevel.Debug, "Starting Neo4J Benchmark");
            return await _neo4JBenchmarkService.StartUserBenchmarkAsync();
        }
    }
}