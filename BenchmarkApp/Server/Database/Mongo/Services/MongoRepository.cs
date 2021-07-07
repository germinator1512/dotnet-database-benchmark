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

        public async Task<IEnumerable<MongoFriendShipEntity>> GetAllFriendsAsync(int level)
        {
            var user = await _ctx.Users
                .Find(u => u.Name == "Max Mustermann")
                .SingleAsync();

            await user.LoadFriendShips(_ctx);
            foreach (var entity in user.FriendShips)
            {
                await entity.LoadFriend(_ctx);
            }

            if (level > 0)
            {
                
            }

            return user.FriendShips;
        }
    }
}