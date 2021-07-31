using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.SQL.Interfaces;
using BenchmarkApp.Server.Services.Interfaces;
using BenchmarkApp.Shared;
using BenchmarkApp.Server.Database.Core;

namespace BenchmarkApp.Server.Services
{
    public class SqlBenchmarkService : ISQLBenchmarkService
    {
        private readonly ISqlRepository _sqlRepository;
        public SqlBenchmarkService(ISqlRepository sqlRepository) => _sqlRepository = sqlRepository;


        public async Task<BenchmarkResult> StartBenchmark()
        {
            var timer = new Stopwatch();
            timer.Start();

            var entities = await _sqlRepository.GetAllFriendsAsync(Config.Level);

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