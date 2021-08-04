using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.SQL.Services;
using BenchmarkApp.Server.Services.Interfaces;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Services
{
    public class SqlBenchmarkService : IBenchmarkService<SqlBenchmarkService>
    {
        private readonly IDataLoader<SqlRepository> _sqlRepository;
        public SqlBenchmarkService(IDataLoader<SqlRepository> sqlRepository) => _sqlRepository = sqlRepository;

        public async Task<IEnumerable<BenchmarkResult>> StartFriendsWithNeighboursBenchmarkAsync() =>
            await TimerService.BenchmarkAsync(_sqlRepository, _sqlRepository.LoadNestedEntitiesAsync);

        public async Task<IEnumerable<BenchmarkResult>> StartUserBenchmarkAsync() =>
            await TimerService.BenchmarkAsync(_sqlRepository, _sqlRepository.LoadEntitiesAsync);
    }
}