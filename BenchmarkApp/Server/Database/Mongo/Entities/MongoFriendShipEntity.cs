using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Entities
{
    public class MongoFriendShipEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public MongoDBRef FriendRef { get; set; }

        [BsonIgnore] private MongoUserEntity Friend { get; set; }

        public async Task LoadFriend(MongoDatabaseContext context)
            => Friend = await context.Users
                .Find(f => f.Id == FriendRef.Id)
                .SingleAsync();
    }
}