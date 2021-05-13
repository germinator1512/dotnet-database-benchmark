using System.Threading.Tasks;
using BenchmarkApp.Server.Database.SQL;
using BenchmarkApp.Server.Services.Interfaces;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Services
{
    public class SQLBenchmarkService : ISQLBenchmarkService
    {
        private readonly ISqlRepository _sqlRepository;

        public SQLBenchmarkService(ISqlRepository sqlRepository)
        {
            _sqlRepository = sqlRepository;
        }

        public async Task<BenchmarkResult> StartBenchmark()
        {
            var entities = await _sqlRepository.GetAllEntitiesAsync();
            return new BenchmarkResult
            {
                Success = true
            };
        }
    }
}