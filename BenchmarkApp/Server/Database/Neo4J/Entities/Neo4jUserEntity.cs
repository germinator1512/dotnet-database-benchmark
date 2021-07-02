using System.Collections.Generic;
using BenchmarkApp.Server.Database.Core;

namespace BenchmarkApp.Server.Database.Neo4J.Entities
{
    public class Neo4jUserEntity :IUserEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Neo4jUserEntity> Friends { get; set; }
    }
}