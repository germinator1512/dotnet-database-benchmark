using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Neo4J.Entities;
using BenchmarkApp.Server.Database.Neo4J.Interfaces;
using Neo4j.Driver;

namespace BenchmarkApp.Server.Database.Neo4J.Services
{
    public class Neo4JRepository : INeo4JRepository
    {
        private readonly Neo4JDatabaseContext _ctx;

        public Neo4JRepository(Neo4JDatabaseContext context) => _ctx = context;

        public async Task<IEnumerable<Neo4jUserEntity>> GetAllFriendsAsync(int level)
        {
            const string query =
                "MATCH (user:User {name: $name}) -[:KNOWS] -> (friend:User) return friend";

            var session = _ctx.Driver.AsyncSession();
            var friends = new List<Neo4jUserEntity>();
            try
            {
                var executionResult = await session.RunAsync(query, new {name = "Max Mustermann"});
                var results = await executionResult.ToListAsync();


                foreach (var result in results)
                {
                    var props = result[0].As<INode>().Properties;

                    var user = new Neo4jUserEntity(props);
                    friends.Add(user);
                }

                return friends;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                await session.CloseAsync();
            }

            return friends;
        }
    }
}