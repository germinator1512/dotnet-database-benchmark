using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo.Entities;
using BenchmarkApp.Server.Database.Mongo.Interfaces;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Services
{
    public class MongoRepository : IMongoRepository
    {
        private readonly MongoDatabaseContext _ctx;

        public MongoRepository(MongoDatabaseContext context) => _ctx = context;

        public async Task<IEnumerable<MongoUserEntity>> GetAllEntitiesAsync() =>
            await _ctx.Users.Find(_ => true).ToListAsync();

        public void AddEntities(IEnumerable<MongoUserEntity> users) => _ctx.Users.InsertMany(users);
    }
}