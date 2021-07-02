using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.SQL.Entities;
using BenchmarkApp.Server.Database.SQL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BenchmarkApp.Server.Database.SQL.Services
{
    public class PostgresInitializerService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public PostgresInitializerService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SqlDatabaseContext>();
            await context.Database.MigrateAsync(cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            var repository = scope.ServiceProvider.GetRequiredService<ISqlRepository>();
            var entities = await repository.GetAllEntitiesAsync();
            if (!entities.Any())
            {
                AddDataSet(repository);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private static void AddDataSet(ISqlRepository repository)
        {
            Console.WriteLine("No entities found in PostgresDb - Inserting Test Dataset");


            var userA = new SqlUserEntity
            {
                Name = "Max Mustermann"
            };
            var userB = new SqlUserEntity
            {
                Name = "Erika Mustermann"
            };

            repository.AddEntitiesAsync(new List<SqlUserEntity> {userA, userB});
        }
    }
}