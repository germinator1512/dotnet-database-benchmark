using System.Collections.Generic;

namespace BenchmarkApp.Server.Database.Neo4J.Entities
{
    public class Neo4jUserEntity
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public object ToParameters() => new {name = Name, id = Id};
        
        public IEnumerable<Neo4jUserEntity> Friends { get; set; }
    }
}