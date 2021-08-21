using System;
using BenchmarkApp.Server.Database.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BenchmarkApp.Server.Database.Mongo.Services;
using BenchmarkApp.Server.Database.Neo4J;
using BenchmarkApp.Server.Database.Neo4J.Services;
using BenchmarkApp.Server.Database.SQL.Services;
using BenchmarkApp.Server.Services;
using Microsoft.EntityFrameworkCore;
using Neo4jClient;
using Newtonsoft.Json.Serialization;

namespace BenchmarkApp.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => _configuration = configuration;

        private readonly IConfiguration _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var config = new Neo4JConfig();
            _configuration.GetSection("Neo4JConfig").Bind(config);

            services.AddControllersWithViews();
            services.AddRazorPages();


            services.AddDbContext<SqlDatabaseContext>((_, options) =>
            {
                var connectionString = _configuration.GetConnectionString("Postgres");
                options.UseNpgsql(connectionString)
                    .LogTo(Console.WriteLine)
                    .EnableSensitiveDataLogging();
            });

            var neo4JClient = new GraphClient(new Uri(config.ClientUrl), config.User, config.Password)
            {
                JsonContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            services.AddSingleton<IGraphClient>(neo4JClient);
            services.AddTransient<MongoDatabaseContext>();


            services.AddTransient<IDataRepository<MongoRepository>, MongoRepository>();
            services.AddTransient<IDataRepository<Neo4JRepository>, Neo4JRepository>();
            services.AddTransient<IDataRepository<SqlRepository>, SqlRepository>();

            services.AddTransient<BenchmarkService<MongoRepository>, MongoBenchmarkService>();
            services.AddTransient<BenchmarkService<Neo4JRepository>, Neo4JBenchmarkService>();
            services.AddTransient<BenchmarkService<SqlRepository>, SqlBenchmarkService>();


            services.AddTransient<TimerService>();
            services.AddTransient<FakeDataGeneratorService>();

            // database initialization
            services.AddTransient<PostgresInitializerService>();
            services.AddTransient<MongoInitializerService>();
            services.AddTransient<Neo4JInitializerService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}