using System;
using Neo4jClient.Cypher;

namespace BenchmarkApp.Server.Database.Neo4J
{
    public static class Neo4JCypherExtensions
    {
        public static ICypherFluentQuery If(
            this ICypherFluentQuery source,
            bool condition,
            Func<ICypherFluentQuery, ICypherFluentQuery> transform
        ) => condition ? transform(source) : source;
    }
}