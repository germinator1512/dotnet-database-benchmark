using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BenchmarkApp.Server.Test.Unit
{
    public class UnitFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private IServiceScope _serviceScope;

        public T GetRequiredService<T>()
        {
            return (_serviceScope ??= Services.CreateScope()).ServiceProvider.GetRequiredService<T>();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((s, conf) =>
            {
                var projectDir = Directory.GetCurrentDirectory();
                var configPath = Path.Combine(projectDir, "appsettings.Development.json");
                conf.AddJsonFile(configPath);
            });

            builder.ConfigureServices(services =>
            {
            
            });
        }
    }
}