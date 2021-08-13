using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Services.Interfaces
{
    public class BenchmarkService<T> where T : class, IDataRepository<T>
    {
        protected BenchmarkService(IDataRepository<T> repository) => _repository = repository;

        private readonly IDataRepository<T> _repository;

        public async Task<IEnumerable<BenchmarkResult>> StartFriendsWithNeighboursBenchmarkAsync()
        {
            await _repository.ConnectAsync();
            return await TimerService.BenchmarkAsync(_repository.LoadNestedEntitiesAsync);
        }

        public async Task<IEnumerable<BenchmarkResult>> StartUserBenchmarkAsync()
        {
            await _repository.ConnectAsync();
            return await TimerService.BenchmarkAsync(_repository.LoadEntitiesAsync);
        }

        public async Task<IEnumerable<BenchmarkResult>> StartAggregateBenchmarkAsync()
        {
            await _repository.ConnectAsync();
            return await TimerService.BenchmarkAsync(_repository.LoadAggregateAsync);
        }

        public async Task<IEnumerable<BenchmarkResult>> StartWriteBenchmarkAsync()
        {
            await _repository.ConnectAsync();
            return await TimerService.BenchmarkAsync(_repository.WriteEntitiesAsync);
        }

        public async Task<IEnumerable<BenchmarkResult>> StartNestedWriteBenchmarkAsync()
        {
            await _repository.ConnectAsync();
            return await TimerService.BenchmarkAsync(_repository.WriteNestedEntitiesAsync);
        }
    }
}