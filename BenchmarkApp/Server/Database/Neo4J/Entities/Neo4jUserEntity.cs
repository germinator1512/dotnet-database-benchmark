using System;
using System.Collections.Generic;
using BenchmarkApp.Server.Database.Core;
using Bogus.DataSets;

namespace BenchmarkApp.Server.Database.Neo4J.Entities
{
    public class Neo4JUserEntity : IBaseUser
    {
        public string Id { get; set; }

        public string Identifier { get; set; }

        // Fake User Data
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public Name.Gender Gender { get; set; }

        public object ToMap() => new
        {
            id = Id,
            identifier = Identifier,
            firstName = FirstName,
            lastName = LastName,
            birthday = Birthday,
            email = Email,
            userName = UserName,
            gender = Gender
        };

        public IEnumerable<Neo4JUserEntity> Friends { get; set; }
    }
}