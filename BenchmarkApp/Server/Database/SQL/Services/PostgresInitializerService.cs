using System;
using System.Threading;
using System.Threading.Tasks;
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
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}