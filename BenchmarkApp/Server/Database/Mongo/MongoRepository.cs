using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.SQL.Entities;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo
{
    public class MongoRepository : IMongoRepository
    {
        private readonly MongoDatabaseContext _ctx;

        public MongoRepository(MongoDatabaseContext context) => _ctx = context;

        public async Task<IEnumerable<UserEntity>> GetAllEntitiesAsync()
        {
            return await _ctx.Users.Find(_ => true).ToListAsync();
        }
    }
}