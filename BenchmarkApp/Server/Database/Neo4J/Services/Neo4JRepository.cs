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
            var result = await _client.Cypher
                .Match(@"(user:User{name: $name})-[:KNOWS]->(friend:User)")
                .WithParam("name", Config.RootUserName)
                .Return<Neo4JUserEntity>("friend")
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


        public async Task EmptyDatabase() =>
            await _client.Cypher
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