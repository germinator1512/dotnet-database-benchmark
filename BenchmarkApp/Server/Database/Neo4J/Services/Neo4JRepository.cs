using System;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Neo4J.Entities;
using Neo4jClient;

namespace BenchmarkApp.Server.Database.Neo4J.Services
{
    public class Neo4JRepository : IDataRepository<Neo4JRepository>
    {
        private readonly IGraphClient _client;
        private readonly FakeDataGeneratorService _faker;

        public Neo4JRepository(IGraphClient client, FakeDataGeneratorService faker)
        {
            _client = client;
            _faker = faker;
        }

        public async Task<int> LoadAggregateAsync(int level)
        {
            var howMany = (int) Math.Pow(Config.FriendsPerUser, level + 1);

            var result = (await _client.Cypher
                    .Match("(n:User)")
                    .Return<double>("n.age")
                    .Limit(howMany)
                    .ResultsAsync)
                .Average();

            return howMany;
        }

        public async Task<int> WriteEntitiesAsync(int level)
        {
            var howMany = (int) Math.Pow(Config.FriendsPerUser, level + 1);

            var names = Enumerable.Range(1, howMany).Select(i => Config.UserName(level, i)).ToList();
            var fakeUsers = _faker.GenerateFakeUsers<Neo4JUserEntity>(names).ToList();

            foreach (var userEntity in fakeUsers)
            {
                userEntity.Id = Guid.NewGuid().ToString();
            }

            await _client.Cypher
                .Unwind(fakeUsers, "friend")
                .Merge("(user:WriteUser {" +
                       "identifier: friend.identifier, " +
                       "id: friend.id," +
                       "firstName:friend.firstName," +
                       "lastName:friend.lastName," +
                       "age: friend.age," +
                       "email:friend.email," +
                       "userName:friend.userName," +
                       "gender:friend.gender}" +
                       ")")
                .ExecuteWithoutResultsAsync();

            return howMany;
        }

        public Task<int> WriteNestedEntitiesAsync(int level)
        {
            throw new NotImplementedException();
        }

        public async Task ConnectAsync()
        {
            await _client.ConnectAsync();
            await LoadNestedEntitiesAsync(0);
        }

        public async Task<int> LoadNestedEntitiesAsync(int level)
        {
            var result = await _client.Cypher
                .Match(GenerateQueryString(level))
                .WithParam("name", Config.RootUserName)
                .Return<Neo4JUserEntity>("rootUser")
                .ResultsAsync;

            return (int) Math.Pow(Config.FriendsPerUser, level + 1);
        }

        public async Task<int> LoadEntitiesAsync(int level)
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


        public async Task EmptyWriteDatabaseAsync()
            => await _client.Cypher
                .Match("(n:WriteUser)")
                .DetachDelete("n")
                .ExecuteWithoutResultsAsync();

        public async Task<bool> IsReadDatabaseEmptyAsync()
            => (await _client.Cypher
                    .Match("(n:User)")
                    .Return(n => n.Count())
                    .Limit(1)
                    .ResultsAsync)
                .Single() == 0;


        public async Task EmptyReadDatabaseAsync()
            => await _client.Cypher
                .Match("(n:User)")
                .DetachDelete("n")
                .ExecuteWithoutResultsAsync();
    }
}