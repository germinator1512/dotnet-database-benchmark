using System.Collections.Generic;

namespace BenchmarkApp.Server.Database.Neo4J.Entities
{
    public class Neo4JUserEntity
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public object ToMap() => new {name = Name, id = Id};

        public IEnumerable<Neo4JUserEntity> Friends { get; set; }
    }
}