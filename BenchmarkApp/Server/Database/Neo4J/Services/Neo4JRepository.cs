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

        public async Task ConnectAsync()
        {
            await _client.ConnectAsync();
            await GetAllFriendsAsync(0);
        }

        public async Task<IEnumerable<Neo4JUserEntity>> GetAllFriendsAsync(int level)
        {
            var result = await _client.Cypher
                .Match(GenerateQueryString(level))
                .WithParam("name", Config.RootUserName)
                .Return<Neo4JUserEntity>("rootUser")
                .ResultsAsync;

            return result.Single().Friends;
        }

        public async Task<IEnumerable<Neo4JUserEntity>> GetUserAsync(int howMany)
        {
            var result = await _client.Cypher
                .Match("(user:User)")
                .Return<Neo4JUserEntity>("user")
                .Limit(howMany)
                .ResultsAsync;

            return result;
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