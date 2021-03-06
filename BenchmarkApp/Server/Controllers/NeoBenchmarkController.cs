using BenchmarkApp.Server.Database.Neo4J.Services;
using BenchmarkApp.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BenchmarkApp.Server.Controllers
{
    [ApiController]
    [Route("benchmark/neo4j")]
    public class NeoBenchmarkController : BenchmarkController<Neo4JRepository>
    {
        public NeoBenchmarkController(BenchmarkService<Neo4JRepository> service,
            ILogger<Neo4JRepository> logger) : base(service, logger)
        {
        }
    }
}