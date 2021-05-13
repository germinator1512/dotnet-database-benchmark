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

        [HttpGet("mongo")]
        public async Task<BenchmarkResult> Mongo()
        {
            _logger.Log(LogLevel.Debug, "Starting Mongo Benchmark");
            return await _mongoBenchmarkService.StartBenchmark();
        }

        [HttpGet("sql")]
        public async Task<BenchmarkResult> Sql()
        {
            _logger.Log(LogLevel.Debug, "Starting SQL Benchmark");
            return await _sqlBenchmarkService.StartBenchmark();
        }

        [HttpGet("neo4j")]
        public async Task<BenchmarkResult> Neo4J()
        {
            _logger.Log(LogLevel.Debug, "Starting Neo4J Benchmark");
            return await _neo4JBenchmarkService.StartBenchmark();
        }
    }
}