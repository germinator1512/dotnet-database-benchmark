using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Neo4J.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo4jClient;

namespace BenchmarkApp.Server.Database.Neo4J.Services
{
    public class Neo4JInitializerService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private IGraphClient _client;
        
        public Neo4JInitializerService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            _client = scope.ServiceProvider.GetRequiredService<IGraphClient>();

            await EmptyDatabase();

            var isEmpty = await IsDatabaseEmpty();
            if (isEmpty) await AddDataSet();
        }


        private async Task<bool> IsDatabaseEmpty()
            => (await _client.Cypher
                    .Match("(n:User)")
                    .Return(n => n.Count())
                    .Limit(1)
                    .ResultsAsync)
                .Single() == 0;


        private async Task EmptyDatabase() =>
            await _client.Cypher
                .Match("(n)")
                .DetachDelete("n")
                .ExecuteWithoutResultsAsync();
        
        private async Task InsertSingleUser(Neo4JUserEntity single)
            => await _client.Cypher
                .Create("(user:User {name: $name, id: $id})")
                .WithParams(single.ToMap())
                .ExecuteWithoutResultsAsync();

        private async Task InsertUsersAsFriends(Neo4JUserEntity rootUser, IEnumerable<Neo4JUserEntity> friends)
            => await _client.Cypher
                .Match("(root:User)")
                .Where((Neo4JUserEntity root) => root.Id == rootUser.Id)
                .Unwind(friends, "friend")
                .Merge("(user:User {name: friend.name, id: friend.id}) <-[:KNOWS]-(root)")
                .WithParam("rootId", rootUser.Id)
                .ExecuteWithoutResultsAsync();

        private async Task AddDataSet()
        {
            Console.WriteLine("No entities found in Neo4J - Inserting Test Dataset");

            const string firstName = "Max Mustermann";
            var firstUser = new Neo4JUserEntity
            {
                Name = firstName,
                Id = Guid.NewGuid().ToString()
            };

            await InsertSingleUser(firstUser);

            var level1Friends = GenerateFriends(9, 1);
            await InsertUsersAsFriends(firstUser, level1Friends);


            // foreach (var level1Friend in level1Friends)
            // {
            //     await InsertFriends(session, firstUser, level1Friend);
            //     var level2Friends = GenerateFriends(10, 2);
            //
            //     foreach (var level2Friend in level2Friends)
            //     {
            //         await InsertFriends(session, level1Friend, level2Friend);
            //         var level3Friends = GenerateFriends(10, 3);
            //
            //         foreach (var level3Friend in level3Friends)
            //         {
            //             await InsertFriends(session, level2Friend, level3Friend);
            //             var level4Friends = GenerateFriends(10, 4);
            //
            //             foreach (var level4Friend in level4Friends)
            //             {
            //                 await InsertFriends(session, level3Friend, level4Friend);
            //                 var level5Friends = GenerateFriends(10, 5);
            //
            //                 foreach (var level5Friend in level5Friends)
            //                 {
            //                     await InsertFriends(session, level4Friend, level5Friend);
            //                     var level6Friends = GenerateFriends(10, 6);
            //                 }
            //             }
            //         }
            //     }
            // }
        }



        private static IEnumerable<Neo4JUserEntity> GenerateFriends(
            int howMany,
            int level)
        {
            var newFriends = new List<Neo4JUserEntity>();

            for (var z = 1; z <= howMany; z++)
            {
                var friend = new Neo4JUserEntity
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