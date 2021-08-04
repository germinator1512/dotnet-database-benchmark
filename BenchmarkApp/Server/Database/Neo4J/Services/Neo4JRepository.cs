using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Neo4J.Entities;
using Neo4jClient;

namespace BenchmarkApp.Server.Database.Neo4J.Services
{
    public class Neo4JRepository : IDataLoader<Neo4JRepository>
    {
        private readonly IGraphClient _client;
        public Neo4JRepository(IGraphClient client) => _client = client;

        public async Task ConnectAsync()
        {
            await _client.ConnectAsync();
            await LoadNestedEntities(0);
        }

        public async Task<int> LoadNestedEntities(int level)
        {
            var result = await _client.Cypher
                .Match(GenerateQueryString(level))
                .WithParam("name", Config.RootUserName)
                .Return<Neo4JUserEntity>("rootUser")
                .ResultsAsync;

            return (int) Math.Pow(Config.FriendsPerUser, level + 1);
        }

        public async Task<int> LoadEntities(int level)
        {
            var howMany = (int) Math.Pow(Config.FriendsPerUser, level + 1);

            var result = await _client.Cypher
                .Match("(user:User)")
                .Return<Neo4JUserEntity>("user")
                .Limit(howMany)
                .ResultsAsync;

            return result.Count();
        }


        private static string GenerateQueryString(int level)
        {
            var queryString = @"(user:User{name:$name})-[:KNOWS]->(friend1:User) ";

            foreach (var i in Enumerable.Range(1, level))
            {
                queryString += ($"-[:KNOWS]->(friend{i + 1}:User) ");
            }

            foreach (var depth in Enumerable.Range(0, level + 1).Reverse())
            {
                var withQuery = "with user,";

                foreach (var userIndex in Enumerable.Range(1, depth))
                {
                    withQuery += ($"friend{userIndex}, ");
                }

                withQuery += $"{{name: friend{depth + 1}.name, id: friend{depth + 1}.id ";

                if (depth != level)
                {
                    withQuery += $", friends: collect(friends{depth + 2})";
                }

                withQuery += $"}} as friends{depth + 1} ";

                queryString += withQuery;
            }

            return queryString + "with {name:user.name, id:user.id, friends: collect(friends1)} as rootUser";
        }


        public async Task<bool> IsDatabaseEmpty()
            => (await _client.Cypher
                    .Match("(n:User)")
                    .Return(n => n.Count())
                    .Limit(1)
                    .ResultsAsync)
                .Single() == 0;


        public async Task EmptyDatabase()
            => await _client.Cypher
                .Match("(n)")
                .DetachDelete("n")
                .ExecuteWithoutResultsAsync();
    }
}