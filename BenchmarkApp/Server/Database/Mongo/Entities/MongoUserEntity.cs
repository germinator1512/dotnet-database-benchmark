using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Entities
{
    public class MongoUserEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; init; }

        [BsonIgnore]
        public IList<MongoFriendShipEntity> FriendShips { get; private set; } = new List<MongoFriendShipEntity>();

        public async Task LoadFriendShips(MongoDatabaseContext context)
            => FriendShips = await context.FriendShips
                .Find(f => f.FriendARef.Id == Id)
                .ToListAsync();
    }
}