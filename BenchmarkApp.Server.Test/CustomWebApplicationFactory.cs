using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Mongo.Services;
using BenchmarkApp.Server.Database.Neo4J.Services;
using BenchmarkApp.Server.Database.SQL.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BenchmarkApp.Server.Test
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var mongoRepoMock = new Mock<IDataRepository<MongoRepository>>();
                services.AddSingleton(mongoRepoMock);
                services.AddSingleton(mongoRepoMock.Object);

                var sqlRepoMock = new Mock<IDataRepository<SqlRepository>>();
                services.AddSingleton(sqlRepoMock);
                services.AddSingleton(sqlRepoMock.Object);

                var neoRepoMock = new Mock<IDataRepository<Neo4JRepository>>();
                services.AddSingleton(neoRepoMock);
                services.AddSingleton(neoRepoMock.Object);
            });
        }
    }
}