using System.Collections.Generic;
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

        public List<MongoDBRef> Friends { get; set; }
    }
}