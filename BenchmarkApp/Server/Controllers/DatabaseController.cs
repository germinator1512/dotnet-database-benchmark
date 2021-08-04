using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo.Services;
using BenchmarkApp.Server.Database.Neo4J.Services;
using BenchmarkApp.Server.Database.SQL.Services;
using BenchmarkApp.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BenchmarkApp.Server.Controllers
{
    [ApiController]
    [Route("database")]
    public class DatabaseController : ControllerBase
    {
        private readonly ILogger<BenchmarkController> _logger;
        private readonly Neo4JInitializerService _neo4JInitializerService;
        private readonly MongoInitializerService _mongoInitializerService;
        private readonly PostgresInitializerService _postgresService;

        public DatabaseController(ILogger<BenchmarkController> logger,
            PostgresInitializerService postgresService,
            Neo4JInitializerService neo4JInitializerService,
            MongoInitializerService mongoInitializerService)
        {
            _logger = logger;
            _postgresService = postgresService;
            _neo4JInitializerService = neo4JInitializerService;
            _mongoInitializerService = mongoInitializerService;
        }

        [HttpGet("mongo")]
        public async Task<InsertResult> Mongo()
        {
            _logger.Log(LogLevel.Debug, "Starting Mongo Data Insertion");
            return await _mongoInitializerService.InsertUserDataSetAsync();
        }

        [HttpGet("sql")]
        public async Task<InsertResult> Sql()
        {
            _logger.Log(LogLevel.Debug, "Starting SQL Data Insertion");
            return await _postgresService.InsertUserDataSetAsync();
        }

        [HttpGet("neo4j")]
        public async Task<InsertResult> Neo4J()
        {
            _logger.Log(LogLevel.Debug, "Starting Neo4J Data Insertion");
            return await _neo4JInitializerService.InsertUserDataSetAsync();
        }
    }
}