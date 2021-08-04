using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Mongo.Services;
using BenchmarkApp.Server.Services.Interfaces;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Services
{
    public class MongoBenchmarkService : IBenchmarkService<MongoBenchmarkService>
    {
        private readonly IDataLoader<MongoRepository> _mongoRepository;

        public MongoBenchmarkService(IDataLoader<MongoRepository> mongoRepository) =>
            _mongoRepository = mongoRepository;

        public async Task<IEnumerable<BenchmarkResult>> StartFriendsWithNeighboursBenchmarkAsync() =>
            await TimerService.BenchmarkAsync(_mongoRepository, _mongoRepository.LoadNestedEntitiesAsync);

        public async Task<IEnumerable<BenchmarkResult>> StartUserBenchmarkAsync() =>
            await TimerService.BenchmarkAsync(_mongoRepository, _mongoRepository.LoadEntitiesAsync);
    }
}