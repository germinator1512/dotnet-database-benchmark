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

        public string Name { get; set; }

        public IEnumerable<MongoDBRef> FriendShipRefs { get; } = new List<MongoDBRef>();

        [BsonIgnore] public IList<MongoFriendShipEntity> FriendShips { get; set; } = new List<MongoFriendShipEntity>();


        public async Task LoadFriendShips(MongoDatabaseContext context)
        {
            var temp = new List<MongoFriendShipEntity>();
            foreach (var friend in FriendShipRefs)
            {
                var entity = await context.FriendShips
                    .Find(f => f.Id == friend.Id)
                    .SingleAsync();

                FriendShips.Add(entity);
            }

            FriendShips = temp;
        }
    }
}