using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo.Entities;

namespace BenchmarkApp.Server.Database.Mongo.Interfaces
{
    public interface IMongoRepository
    {
        Task<IEnumerable<MongoUserEntity>> GetAllFriendsAsync(int level);
        
        Task<bool> IsDatabaseEmpty(CancellationToken cancellationToken);

        Task EmptyDatabase(CancellationToken cancellationToken);
    }
}