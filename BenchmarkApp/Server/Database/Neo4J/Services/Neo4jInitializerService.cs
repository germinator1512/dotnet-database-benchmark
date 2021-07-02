using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Neo4J.Entities;
using BenchmarkApp.Server.Database.Neo4J.Interfaces;
using BenchmarkApp.Server.Database.SQL.Entities;
using BenchmarkApp.Server.Database.SQL.Interfaces;
using BenchmarkApp.Server.Database.SQL.Services;
using Microsoft.EntityFrameworkCore;
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
            var repository = scope.ServiceProvider.GetRequiredService<INeo4JRepository>();
            var entities = await repository.GetAllEntitiesAsync();
            if (!entities.Any())
            {
                AddDataSet(repository);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

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

            repository.AddEntitiesAsync(new List<Neo4jUserEntity> {userA, userB});
        }
    }
}