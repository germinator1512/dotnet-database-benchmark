using System;
using System.Collections.Generic;
using System.Linq;
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
            return await BenchmarkAsync(_repository.LoadNestedEntitiesAsync);
        }

        public async Task<IEnumerable<BenchmarkResult>> StartUserBenchmarkAsync()
        {
            await _repository.ConnectAsync();
            return await BenchmarkAsync(_repository.LoadEntitiesAsync);
        }

        public async Task<IEnumerable<BenchmarkResult>> StartAggregateBenchmarkAsync()
        {
            await _repository.ConnectAsync();
            return await BenchmarkAsync(_repository.LoadAggregateAsync);
        }

        public async Task<IEnumerable<BenchmarkResult>> StartWriteBenchmarkAsync()
        {
            await _repository.ConnectAsync();
            await _repository.EmptyWriteDatabaseAsync();
            return await BenchmarkAsync(_repository.WriteEntitiesAsync);
        }

        public async Task<IEnumerable<BenchmarkResult>> StartNestedWriteBenchmarkAsync()
        {
            await _repository.ConnectAsync();
            await _repository.EmptyWriteDatabaseAsync();
            return await BenchmarkAsync(_repository.WriteNestedEntitiesAsync);
        }

        /// <summary>
        /// executes the given function with 0 .. Config.NestedUsers as parameters
        /// </summary>
        /// <param name="benchmarkFunction">function to execute </param>
        /// <returns>List of Benchmark-results of given function</returns>
        private static async Task<IEnumerable<BenchmarkResult>> BenchmarkAsync(
            Func<int, Task<int>> benchmarkFunction)
        {
            var results = new List<BenchmarkResult>();
            foreach (var level in Enumerable.Range(0, Config.NestedUserLevels))
            {
                var result = await TimerService.ExecuteBenchmarkAsync(benchmarkFunction, level);
                results.Add(result);
            }

            return results;
        }
    }
}