using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Neo4J.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo4jClient;

namespace BenchmarkApp.Server.Database.Neo4J.Services
{
    public class Neo4JInitializerService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private Neo4JRepository _repository;

        public Neo4JInitializerService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            _repository = scope.ServiceProvider.GetRequiredService<Neo4JRepository>();

            var client =  scope.ServiceProvider.GetRequiredService<IGraphClient>();
            await client.ConnectAsync();

            await _repository.EmptyDatabase();

            var isEmpty = await _repository.IsDatabaseEmpty();
            if (isEmpty) await AddDataSet();
        }


        private async Task AddDataSet()
        {
            Console.WriteLine("No entities found in Neo4J - Inserting Test Dataset");

            const string firstName = EntityConfig.RootUserName;
            var firstUser = new Neo4JUserEntity
            {
                Name = firstName,
                Id = Guid.NewGuid().ToString()
            };

            await  _repository.InsertSingleUser(firstUser);


            var level1Friends = GenerateFriends(9, 1);
            await  _repository.InsertUsersAsFriends(firstUser, level1Friends);

            foreach (var level1Friend in level1Friends)
            {
                var level2Friends = GenerateFriends(10, 2);
                await  _repository.InsertUsersAsFriends(level1Friend, level2Friends);

                foreach (var level2Friend in level2Friends)
                {
                    var level3Friends = GenerateFriends(10, 3);
                    await  _repository.InsertUsersAsFriends(level2Friend, level3Friends);

                    foreach (var level3Friend in level3Friends)
                    {
                        var level4Friends = GenerateFriends(10, 4);
                        await  _repository.InsertUsersAsFriends(level3Friend, level4Friends);

                        foreach (var level4Friend in level4Friends)
                        {
                            var level5Friends = GenerateFriends(10, 5);
                            await  _repository.InsertUsersAsFriends(level4Friend, level5Friends);

                            foreach (var level5Friend in level5Friends)
                            {
                                var level6Friends = GenerateFriends(10, 6);
                                await  _repository.InsertUsersAsFriends(level5Friend, level6Friends);
                            }
                        }
                    }
                }
            }
        }

        private static IEnumerable<Neo4JUserEntity> GenerateFriends(
            int howMany,
            int level)
            => Enumerable
                .Range(1, howMany)
                .Select(z => new Neo4JUserEntity
                {
                    Name = EntityConfig.UserName(level, z),
                    Id = Guid.NewGuid().ToString()
                }).ToList();


        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}