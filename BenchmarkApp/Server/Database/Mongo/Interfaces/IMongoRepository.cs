using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo.Entities;

namespace BenchmarkApp.Server.Database.Mongo.Interfaces
{
    public interface IMongoRepository
    {
        Task<IEnumerable<MongoUserEntity>> GetAllFriendsAsync(int level);

        Task<IEnumerable<MongoUserEntity>> GetUserAsync(int howMany);
        
        Task ConnectAsync();
        
        Task<bool> IsDatabaseEmpty();

        Task EmptyDatabase();
    }
}