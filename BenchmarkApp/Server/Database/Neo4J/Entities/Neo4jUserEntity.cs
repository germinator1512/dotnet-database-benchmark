using System.Collections.Generic;

namespace BenchmarkApp.Server.Database.Neo4J.Entities
{
    public class Neo4jUserEntity 
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<Neo4jUserEntity> Friends { get; set; }
    }
}