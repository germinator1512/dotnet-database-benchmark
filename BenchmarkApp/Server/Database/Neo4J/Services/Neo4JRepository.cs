using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Neo4J.Entities;
using BenchmarkApp.Server.Database.Neo4J.Interfaces;
using Neo4jClient;
using Neo4jClient.Cypher;

namespace BenchmarkApp.Server.Database.Neo4J.Services
{
    public class Neo4JRepository : INeo4JRepository
    {
        private readonly IGraphClient _client;

        public Neo4JRepository(IGraphClient client) => _client = client;

        public async Task<IEnumerable<Neo4JUserEntity>> GetAllFriendsAsync(int level)
        {
            
            // Match (user:User{name:"Max Mustermann"})-[:KNOWS]->(friend:User)
            // -[:KNOWS]->(friend0:User)-[:KNOWS]->(friend1:User)
            // -[:KNOWS]->(friend2:User) return friend, friend0, friend1, friend2
            
            
            var queryString = @"(user:User{name: $name})-[:KNOWS]->(friend:User)";
            var resultString = "friend";

            foreach (var i in Enumerable.Range(0, level))
            {
                queryString += ($"-[:KNOWS]->(friend{i}:User)");
                resultString += ($"friend{i}");
            }

            var result = await _client.Cypher
                .Match(queryString)
                .WithParam("name", Config.RootUserName)
                .Return(() => new
                {
                    Friends = Return.As<Neo4JUserEntity>("friend"),
                    Friends0 = Return.As<Neo4JUserEntity>("friend0"),
                    Friends1 = Return.As<Neo4JUserEntity>("friend1"),
                    Friends2 = Return.As<Neo4JUserEntity>("friend2"),
                })
                .ResultsAsync;


            return new List<Neo4JUserEntity>();
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

        public async Task InsertSingleUser(Neo4JUserEntity single)
            => await _client.Cypher
                .Create("(user:User {name: $name, id: $id})")
                .WithParams(single.ToMap())
                .ExecuteWithoutResultsAsync();

        public async Task InsertUsersAsFriends(Neo4JUserEntity rootUser, IEnumerable<Neo4JUserEntity> friends)
            => await _client.Cypher
                .Match("(root:User)")
                .Where((Neo4JUserEntity root) => root.Id == rootUser.Id)
                .Unwind(friends, "friend")
                .Merge("(user:User {name: friend.name, id: friend.id}) <-[:KNOWS]-(root)")
                .WithParam("rootId", rootUser.Id)
                .ExecuteWithoutResultsAsync();
    }
}