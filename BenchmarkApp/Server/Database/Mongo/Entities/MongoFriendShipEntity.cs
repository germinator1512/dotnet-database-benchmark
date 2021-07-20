using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Entities
{
    public class MongoFriendShipEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public MongoDBRef FriendARef { get; init; }
        public MongoDBRef FriendBRef { get; init; }

        [BsonIgnore] public MongoUserEntity FriendB { get; set; }

        public async Task LoadFriend(MongoDatabaseContext context)
            => FriendB = await context.Users
                .Find(f => f.Id == FriendBRef.Id)
                .SingleAsync();
    }
}