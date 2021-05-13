using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo;
using BenchmarkApp.Server.Services.Interfaces;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Services
{
    public class MongoBenchmarkService : IMongoBenchmarkService
    {
        private readonly IMongoRepository _mongoRepository;

        public MongoBenchmarkService(IMongoRepository mongoRepository)
        {
            _mongoRepository = mongoRepository;
        }


        public async Task<BenchmarkResult> StartBenchmark()
        {
            var entities = await _mongoRepository.GetAllEntitiesAsync();
            return new BenchmarkResult
            {
                Success = true
            };
        }
    }
}