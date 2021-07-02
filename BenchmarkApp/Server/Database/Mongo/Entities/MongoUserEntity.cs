using System.Collections.Generic;
using BenchmarkApp.Server.Database.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Entities
{
    public class MongoUserEntity : IUserEntity
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<MongoDBRef> Friends { get; set; }
    }
}