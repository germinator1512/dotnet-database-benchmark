using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BenchmarkApp.Server.Database.SQL.Entities
{
    public class SqlUserEntity
    {
        [Key] public int Id { get; set; }

        public string Name { get; set; }

        public List<SqlFriendshipEntity> FriendShips { get; set; } = new();
    }
}