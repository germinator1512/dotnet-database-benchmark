using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BenchmarkApp.Server.Database.Core;

namespace BenchmarkApp.Server.Database.SQL.Entities
{
    public class SqlUserEntity : IUserEntity
    {
        [Key] public int Id { get; set; }

        public string Name { get; set; }
        public IEnumerable<SqlUserEntity> Friends { get; set; }
    }
}