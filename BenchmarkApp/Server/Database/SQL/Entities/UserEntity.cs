using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BenchmarkApp.Server.Database.SQL.Entities
{
    public class UserEntity
    {
        [Key] public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<UserEntity> Friends { get; set; }
    }
}