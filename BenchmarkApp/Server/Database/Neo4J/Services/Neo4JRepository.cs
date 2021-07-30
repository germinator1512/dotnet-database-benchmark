using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Neo4J.Entities;
using BenchmarkApp.Server.Database.Neo4J.Interfaces;
using Neo4jClient;

namespace BenchmarkApp.Server.Database.Neo4J.Services
{
    public class Neo4JRepository : INeo4JRepository
    {
        private readonly IGraphClient _client;
        public Neo4JRepository(IGraphClient client) => _client = client;

        public async Task<IEnumerable<Neo4JUserEntity>> GetAllFriendsAsync(int level)
        {
            var queryString = @"(user:User{name:$name})-[:KNOWS]->(friend:User)";

            foreach (var i in Enumerable.Range(0, level))
            {
                queryString += ($"-[:KNOWS]->(friend{i}:User)");
            }

            var result = await _client.Cypher
                .Match(queryString)
                .WithParam("name", Config.RootUserName)
                .If(level > 5, c4 => c4.With(QueryStrings.Friends4)
                    .If(level > 4, c3 => c3.With(QueryStrings.Friends3)
                        .If(level > 3, c2 => c2.With(QueryStrings.Friends2)
                            .If(level > 2, c1 => c1.With(QueryStrings.Friends1)
                                .If(level > 1, c0 => c0.With(QueryStrings.Friends0)
                                )
                            )
                        )
                    )
                )
                .With(QueryStrings.Friends)
                .With(QueryStrings.RootUser)
                .Return<Neo4JUserEntity>("rootUser")
                .ResultsAsync;


            return result.Single().Friends;
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