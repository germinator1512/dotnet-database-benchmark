using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Neo4J.Entities;
using BenchmarkApp.Server.Database.Neo4J.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            
            if (!entities.Any()) AddDataSet(repository);
        }


        private static async Task EmptyDatabase(Neo4JDatabaseContext context)
        {
            var query = "MATCH (n) DETACH DELETE n";
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

        private static void AddDataSet(INeo4JRepository repository)
        {
            Console.WriteLine("No entities found in PostgresDb - Inserting Test Dataset");


            var userA = new Neo4jUserEntity
            {
                Name = "Max Mustermann"
            };
            var userB = new Neo4jUserEntity
            {
                Name = "Erika Mustermann"
            };
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}