using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Mongo.Services;
using Bogus.DataSets;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Entities
{
    public class MongoUserEntity: IBaseUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        public string Identifier { get; set; }

        
        // Fake User Data
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public Name.Gender Gender { get; set; }
        
        
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