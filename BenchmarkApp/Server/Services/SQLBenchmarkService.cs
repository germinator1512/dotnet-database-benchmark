using System.Threading.Tasks;
using BenchmarkApp.Server.Database.SQL.Interfaces;
using BenchmarkApp.Server.Services.Interfaces;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Services
{
    public class SqlBenchmarkService : ISQLBenchmarkService
    {
        private readonly ISqlRepository _sqlRepository;
        public SqlBenchmarkService(ISqlRepository sqlRepository) => _sqlRepository = sqlRepository;


        public async Task<BenchmarkResult> StartBenchmark()
        {
            var entities = await _sqlRepository.GetAllFriendsAsync( 6);
            return new BenchmarkResult
            {
                Success = true
            };
        }
    }
}