using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Mongo.Services;
using BenchmarkApp.Server.Services.Interfaces;

namespace BenchmarkApp.Server.Services
{
    public class MongoBenchmarkService : BenchmarkService<MongoRepository>
    {
        public MongoBenchmarkService(IDataLoader<MongoRepository> loader) : base(loader)
        {
        }
    }
}