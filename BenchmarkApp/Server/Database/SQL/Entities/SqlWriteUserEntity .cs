using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BenchmarkApp.Server.Database.Core;
using Bogus.DataSets;

namespace BenchmarkApp.Server.Database.SQL.Entities
{
    public class SqlWriteUserEntity : IBaseUser
    {
        [Key] public Guid Id { get; set; }

        public string Identifier { get; set; }

        // Fake User Data
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int Age { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public Name.Gender Gender { get; set; }

        [InverseProperty("FriendA")] public List<SqlWriteFriendshipEntity> FriendShips { get; set; } = new();
    }
}