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
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; init; }

        public IEnumerable<MongoDBRef> FriendShipRefs { get; } = new List<MongoDBRef>();

        [BsonIgnore] public IList<MongoFriendShipEntity> FriendShips { get; private set; } = new List<MongoFriendShipEntity>();
        
        public async Task LoadFriendShips(MongoDatabaseContext context)
        {
            var filterDef = new FilterDefinitionBuilder<MongoFriendShipEntity>();
            var filter = filterDef.In(_ => _.Id, FriendShipRefs.Select(f => f.Id));
            FriendShips = await context.FriendShips.Find(filter).ToListAsync();
        }
    }
}