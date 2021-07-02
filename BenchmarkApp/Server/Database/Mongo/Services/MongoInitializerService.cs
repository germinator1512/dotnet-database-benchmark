using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo.Entities;
using BenchmarkApp.Server.Database.Mongo.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Database.Mongo.Services
{
    public class MongoInitializerService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public MongoInitializerService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IMongoRepository>();

            var users = await repository.GetAllEntitiesAsync();
            if (!users.Any())
            {
                AddDataSet(repository);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private void AddDataSet(IMongoRepository repository)
        {
            Console.WriteLine("No entities found in MongoDB - Inserting Test Dataset");

            var userA = new MongoUserEntity
            {
                Name = "Max Mustermann",
                Friends = new List<MongoDBRef>()
            };
            var userB = new MongoUserEntity
            {
                Name = "Erika Mustermann",
                Friends = new List<MongoDBRef> { }
            };

            repository.AddEntities(new List<MongoUserEntity> {userA, userB});
        }
    }
}