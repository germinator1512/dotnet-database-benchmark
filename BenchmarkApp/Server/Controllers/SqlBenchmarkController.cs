using BenchmarkApp.Server.Database.SQL.Services;
using BenchmarkApp.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BenchmarkApp.Server.Controllers
{
    [ApiController]
    [Route("benchmark/sql")]
    public class SqlController : BenchmarkController<SqlRepository>
    {
        public SqlController(BenchmarkService<SqlRepository> service, ILogger<SqlRepository> logger) : base(service, logger)
        {
        }
    }
}