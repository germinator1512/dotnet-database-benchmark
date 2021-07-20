using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo.Services;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Entities
{
    public class MongoUserEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string Name { get; init; }

        [BsonIgnore] public List<MongoUserEntity> Friends { get; private set; } = new();

        public List<MongoDBRef> FriendIds { get; set; } = new();

        public async Task LoadFriends(MongoDatabaseContext context)
        {
            var filterDef = new FilterDefinitionBuilder<MongoUserEntity>();

            Friends = await context.Users
                .Find(filterDef.In(x => x.Id, FriendIds.Select(f => f.Id)))
                .ToListAsync();
        }
    }
}