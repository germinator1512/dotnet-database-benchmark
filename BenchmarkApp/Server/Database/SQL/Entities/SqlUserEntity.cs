using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BenchmarkApp.Server.Database.SQL.Entities
{
    public class SqlUserEntity
    {
        [Key] public Guid Id { get; set; }

        public string Name { get; set; }

        [InverseProperty("FriendA")]
        public List<SqlFriendshipEntity> FriendShips { get; set; } = new();
    }
}