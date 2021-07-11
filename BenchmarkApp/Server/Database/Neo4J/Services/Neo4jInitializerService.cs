using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo.Entities;
using BenchmarkApp.Server.Database.Neo4J.Entities;
using BenchmarkApp.Server.Database.Neo4J.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo4j.Driver;

namespace BenchmarkApp.Server.Database.Neo4J.Services
{
    public class Neo4JInitializerService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public Neo4JInitializerService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Neo4JDatabaseContext>();
            var repository = scope.ServiceProvider.GetRequiredService<INeo4JRepository>();

            var entities = await repository.GetAllEntitiesAsync();

            await EmptyDatabase(context);

            if (!entities.Any())
                await AddDataSet(context);
        }


        private static async Task EmptyDatabase(Neo4JDatabaseContext context)
        {
            const string query = "MATCH (n) DETACH DELETE n";
            var session = context.Driver.AsyncSession();

            try
            {
                var result = await session.RunAsync(query);
                Console.WriteLine(result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                await session.CloseAsync();
            }
        }

        private static async Task AddDataSet(Neo4JDatabaseContext context)
        {
            Console.WriteLine("No entities found in Neo4J - Inserting Test Dataset");


            var session = context.Driver.AsyncSession();
            const string createUserQuery = "CREATE (user:User {name: $name}) RETURN user, id(user) as userId";
            const string createRelationShipQuery =
                "MATCH(a:User),(b:User) WHERE a.name = 'A' AND b.name = 'B' CREATE (a)-[r:KNOWS {name: a.name + '->' + b.name}]->(b) RETURN type(r), r.name";
            try
            {
                await session.WriteTransactionAsync(
                    async tx => { await tx.RunAsync(createUserQuery, new {name = "Max Mustermann"}); });


                var level1Friends = GenerateFriends(9, 1);
                foreach (var level1Friend in level1Friends)
                {
                    await session.WriteTransactionAsync(
                        async tx => { await tx.RunAsync(createUserQuery, new {name = level1Friend.Name}); });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                await session.CloseAsync();
            }


            // await context.Users.AddRangeAsync(level1Friends);
            //
            // foreach (var level1Friend in level1Friends)
            // {
            //     var level2Friends = GenerateFriends(level1Friend, 10, 2);
            //     await context.Users.AddRangeAsync(level2Friends);
            //
            //     foreach (var level2Friend in level2Friends)
            //     {
            //         var level3Friends = GenerateFriends(level2Friend, 10, 3);
            //         await context.Users.AddRangeAsync(level3Friends);
            //     
            //         foreach (var level3Friend in level3Friends)
            //         {
            //             var level4Friends = GenerateFriends(level3Friend, 10, 4);
            //             await context.Users.AddRangeAsync(level4Friends);
            //     
            //             foreach (var level4Friend in level4Friends)
            //             {
            //                 var level5Friends = GenerateFriends(level4Friend, 10, 5);
            //                 await context.Users.AddRangeAsync(level5Friends);
            //     
            //                 foreach (var level5Friend in level5Friends)
            //                 {
            //                     var level6Friends = GenerateFriends(level5Friend, 10, 6);
            //                     await context.Users.AddRangeAsync(level6Friends);
            //                 }
            //             }
            //         }
            //     }
            // }
            //
        }

        private static IEnumerable<Neo4jUserEntity> GenerateFriends(
            int howMany,
            int level)
        {
            var newFriends = new List<Neo4jUserEntity>();

            for (var z = 1; z <= howMany; z++)
            {
                var friend = new Neo4jUserEntity
                {
                    Name = $"Level {level} Friend {z}",
                };

                newFriends.Add(friend);
            }

            return newFriends;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}