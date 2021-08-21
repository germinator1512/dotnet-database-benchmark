using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Services;
using BenchmarkApp.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BenchmarkApp.Server.Controllers
{
    [ApiController]
    public abstract class BenchmarkController<T> where T: class, IDataRepository<T>
    {
        private readonly BenchmarkService<T> _benchmarkService;
        private readonly ILogger<T> _logger;

        protected BenchmarkController(BenchmarkService<T> benchmarkService, ILogger<T> logger)
        {
            _benchmarkService = benchmarkService;
            _logger = logger;
        }


        [HttpGet("neighbour")]
        public async Task<IEnumerable<BenchmarkResult>> Neighbour()
        {
            _logger.Log(LogLevel.Debug, "Starting Neighbour Benchmark");
            return await _benchmarkService.StartFriendsWithNeighboursBenchmarkAsync();
        }

        [HttpGet("user")]
        public async Task<IEnumerable<BenchmarkResult>> User()
        {
            _logger.Log(LogLevel.Debug, "Starting User Benchmark");
            return await _benchmarkService.StartUserBenchmarkAsync();
        }

        [HttpGet("aggregate")]
        public async Task<IEnumerable<BenchmarkResult>> Aggregate()
        {
            _logger.Log(LogLevel.Debug, "Starting Aggregate Benchmark");
            return await _benchmarkService.StartAggregateBenchmarkAsync();
        }

        [HttpGet("writeNested")]
        public async Task<IEnumerable<BenchmarkResult>> WriteNested()
        {
            _logger.Log(LogLevel.Debug, "Starting WriteNested Benchmark");
            return await _benchmarkService.StartNestedWriteBenchmarkAsync();
        }

        [HttpGet("write")]
        public async Task<IEnumerable<BenchmarkResult>> Write()
        {
            _logger.Log(LogLevel.Debug, "Starting Write Benchmark");
            return await _benchmarkService.StartWriteBenchmarkAsync();
        }
    }
}