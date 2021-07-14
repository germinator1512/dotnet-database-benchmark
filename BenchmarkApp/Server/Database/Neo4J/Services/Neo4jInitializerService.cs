using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        private const string CreateUserQuery =
            "CREATE (user:User {name: $name, id: $id}) RETURN user, id(user) as userId";

        private const string CreateRelQuery =
            "MATCH(a:User),(b:User) WHERE a.id = $idA AND b.id = $idB CREATE (a)-[r:KNOWS {name: a.name + '->' + b.name}]->(b) RETURN type(r), r.name";


        private const string EmptyDbQuery = "MATCH (n) DETACH DELETE n";

        private const string IsDbEmptyQuery = "MATCH (n) RETURN count(n) as count";

        public Neo4JInitializerService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<Neo4JDatabaseContext>();
            var repository = scope.ServiceProvider.GetRequiredService<INeo4JRepository>();


            // await EmptyDatabase(context);

            var isEmpty = await IsDatabaseEmpty(context);
            if (isEmpty) await AddDataSet(context);
        }


        private static async Task<bool> IsDatabaseEmpty(Neo4JDatabaseContext context)
        {
            var session = context.Driver.AsyncSession();

            try
            {
                var result = await session.ReadTransactionAsync(async tx =>
                {
                    var reader = await tx.RunAsync(IsDbEmptyQuery);

                    var count = 0.0;
                    while (await reader.FetchAsync())
                    {
                        count = (long) reader.Current[0];
                    }

                    return count;
                });

                Console.WriteLine(result);
                return result == 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                await session.CloseAsync();
            }

            return false;
        }

        private static async Task EmptyDatabase(Neo4JDatabaseContext context)
        {
            var session = context.Driver.AsyncSession();

            try
            {
                var result = await session.RunAsync(EmptyDbQuery);
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

            const string firstName = "Max Mustermann";
            var firstUser = new Neo4jUserEntity
            {
                Name = firstName,
                Id = Guid.NewGuid().ToString()
            };

            try
            {
                await session.WriteTransactionAsync(async tx =>
                {
                    await tx.RunAsync(CreateUserQuery, firstUser.ToParameters());
                });


                var level1Friends = GenerateFriends(9, 1);


                foreach (var level1Friend in level1Friends)
                {
                    await InsertFriends(session, firstUser, level1Friend);
                    var level2Friends = GenerateFriends(10, 2);

                    foreach (var level2Friend in level2Friends)
                    {
                        await InsertFriends(session, level1Friend, level2Friend);
                        var level3Friends = GenerateFriends(10, 3);

                        foreach (var level3Friend in level3Friends)
                        {
                            await InsertFriends(session, level2Friend, level3Friend);
                            var level4Friends = GenerateFriends(10, 4);

                            foreach (var level4Friend in level4Friends)
                            {
                                await InsertFriends(session, level3Friend, level4Friend);
                                var level5Friends = GenerateFriends(10, 5);

                                foreach (var level5Friend in level5Friends)
                                {
                                    await InsertFriends(session, level4Friend, level5Friend);
                                    var level6Friends = GenerateFriends(10, 6);
                                }
                            }
                        }
                    }
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
        }

        private static async Task InsertFriends(
            IAsyncSession session,
            Neo4jUserEntity root,
            Neo4jUserEntity friend)
        {
            await session.WriteTransactionAsync(async tx =>
                await tx.RunAsync(CreateUserQuery, friend.ToParameters()));

            await session.WriteTransactionAsync(async tx =>
                await tx.RunAsync(CreateRelQuery, new {idA = root.Id, idB = friend.Id}));
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
                    Id = Guid.NewGuid().ToString()
                };

                newFriends.Add(friend);
            }

            return newFriends;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}