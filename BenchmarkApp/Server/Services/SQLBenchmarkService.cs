using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.SQL.Services;

namespace BenchmarkApp.Server.Services
{
    public class SqlBenchmarkService : BenchmarkService<SqlRepository>
    {
        public SqlBenchmarkService(IDataRepository<SqlRepository> repository) : base(repository)
        {
        }
    }
}