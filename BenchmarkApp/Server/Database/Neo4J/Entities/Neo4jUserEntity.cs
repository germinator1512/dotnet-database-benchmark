using System.Collections.Generic;

namespace BenchmarkApp.Server.Database.Neo4J.Entities
{
    public class Neo4jUserEntity
    {
        public Neo4jUserEntity(IReadOnlyDictionary<string, object> props)
        {
            Name = props["name"].ToString();
            Id = props["id"].ToString();
            Friends = new List<Neo4jUserEntity>();
        }

        public Neo4jUserEntity(string name, string id)
        {
            Name = name;
            Id = id;
        }
        public string Id { get; set; }

        public string Name { get; set; }

        public object ToParameters() => new {name = Name, id = Id};
        
        public IEnumerable<Neo4jUserEntity> Friends { get; set; }
    }
}