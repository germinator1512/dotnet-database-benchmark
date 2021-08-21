using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Mongo.Services;

namespace BenchmarkApp.Server.Services
{
    public class MongoBenchmarkService : BenchmarkService<MongoRepository>
    {
        public MongoBenchmarkService(IDataRepository<MongoRepository> repository) : base(repository)
        {
        }
    }
}