using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Services.Interfaces
{
    public abstract class BenchmarkService<T>
    {
        protected BenchmarkService(IDataLoader<T> loader) => _loader = loader;

        private readonly IDataLoader<T> _loader;

        public async Task<IEnumerable<BenchmarkResult>> StartFriendsWithNeighboursBenchmarkAsync()
        {
            await _loader.ConnectAsync();
            return await TimerService.BenchmarkAsync(_loader.LoadNestedEntitiesAsync);
        }

        public async Task<IEnumerable<BenchmarkResult>> StartUserBenchmarkAsync()
        {
            await _loader.ConnectAsync();
            return await TimerService.BenchmarkAsync(_loader.LoadEntitiesAsync);
        }

        public async Task<IEnumerable<BenchmarkResult>> StartAggregateBenchmarkAsync()
        {
            await _loader.ConnectAsync();
            return await TimerService.BenchmarkAsync(_loader.LoadAggregateAsync);
        }
    }
}