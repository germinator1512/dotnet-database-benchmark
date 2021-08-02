using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Neo4jClient;

namespace BenchmarkApp.Server.Database.Neo4J.Services
{
    public class Neo4JInitializerService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public Neo4JInitializerService(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = _serviceProvider.CreateScope();
            // _repository = scope.ServiceProvider.GetRequiredService<INeo4JRepository>();

            var client = scope.ServiceProvider.GetRequiredService<IGraphClient>();
            await client.ConnectAsync();

            // await _repository.EmptyDatabase();
            //
            // var isEmpty = await _repository.IsDatabaseEmpty();
            // if (isEmpty) await AddDataSet();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}