using System.Diagnostics;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Mongo.Interfaces;
using BenchmarkApp.Server.Services.Interfaces;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Services
{
    public class MongoBenchmarkService : IMongoBenchmarkService
    {
        private readonly IMongoRepository _mongoRepository;
        public MongoBenchmarkService(IMongoRepository mongoRepository) => _mongoRepository = mongoRepository;

        public async Task<BenchmarkResult> StartBenchmark()
        {
            var timer = new Stopwatch();
            timer.Start();

            var entities = await _mongoRepository.GetAllFriendsAsync(Config.Level);

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