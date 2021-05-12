using System;
using Microsoft.Extensions.Configuration;
using Neo4j.Driver;

namespace BenchmarkApp.Server.Database.Neo4J
{
    public class Neo4JDatabaseContext : IDisposable
    {
        public readonly IDriver Driver;

        public Neo4JDatabaseContext(IConfiguration configuration)
        {
            var config = new Neo4JConfig();
            configuration.GetSection("Neo4JConfig").Bind(config);
            Driver = GraphDatabase.Driver(config.Url, AuthTokens.Basic(config.User, config.Password));
        }

        public void Dispose() => Driver?.Dispose();
    }
}