using BenchmarkApp.Server.Database.Mongo.Services;
using BenchmarkApp.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BenchmarkApp.Server.Controllers
{
    [ApiController]
    [Route("benchmark/mongo")]
    public class MongoBenchmarkController : BenchmarkController<MongoRepository>
    {
        public MongoBenchmarkController(BenchmarkService<MongoRepository> service, ILogger<MongoRepository> logger) :
            base(service, logger)
        {
        }
    }
}