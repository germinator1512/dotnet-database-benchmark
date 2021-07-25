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
            var queryString = @"(user:User{name:$name})
                -[:KNOWS]->(friend:User) 
                -[:KNOWS]->(friend0:User)
                -[:KNOWS]->(friend1:User)
                -[:KNOWS]->(friend2:User)
                -[:KNOWS]->(friend3:User)";


            // foreach (var i in Enumerable.Range(0, level))
            // {
            //     queryString += ($"-[:KNOWS]->(friend{i}:User) ");
            // }

            var result = await _client.Cypher
                .Match(queryString)
                .WithParam("name", Config.RootUserName)
                .With("user, friend, friend0, friend1, friend2, {name:friend3.name, id:friend3.id} as friends3")
                .With("user,friend, friend0, friend1, {name:friend2.name, id:friend2.id, friends: collect(friends3)} as friends2")
                .With("user,friend, friend0, {name:friend1.name, id:friend1.id, friends: collect(friends2)} as friends1")
                .With("user, friend, {name:friend0.name, id:friend0.id, friends: collect(friends1)} as friends0")
                .With("user, {name:friend.name, id:friend.id, friends: collect(friends0)} as friends")
                .With("{name:user.name, id:user.id, friends: collect(friends)} as rootUser")
                .Return<Neo4JUserEntity>("rootUser")
                .ResultsAsync;


            return result;
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