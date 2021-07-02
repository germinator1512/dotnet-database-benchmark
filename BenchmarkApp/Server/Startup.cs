using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BenchmarkApp.Server.Database.Mongo.Interfaces;
using BenchmarkApp.Server.Database.Mongo.Services;
using BenchmarkApp.Server.Database.Neo4J.Interfaces;
using BenchmarkApp.Server.Database.Neo4J.Services;
using BenchmarkApp.Server.Database.SQL.Interfaces;
using BenchmarkApp.Server.Database.SQL.Services;
using BenchmarkApp.Server.Services;
using BenchmarkApp.Server.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BenchmarkApp.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly IConfiguration _configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();


            services.AddDbContext<SqlDatabaseContext>((_, options) =>
            {
                var connectionString = _configuration.GetConnectionString("Postgres");
                options.UseNpgsql(connectionString)
                    .LogTo(Console.WriteLine)
                    .EnableSensitiveDataLogging();
            });

            services.AddTransient<Neo4JDatabaseContext>();
            services.AddTransient<MongoDatabaseContext>();

            services.AddTransient<IMongoRepository, MongoRepository>();
            services.AddTransient<INeo4JRepository, Neo4JRepository>();
            services.AddTransient<ISqlRepository, SqlRepository>();

            services.AddTransient<IMongoBenchmarkService, MongoBenchmarkService>();
            services.AddTransient<INeo4JBenchmarkService, Neo4JBenchmarkService>();
            services.AddTransient<ISQLBenchmarkService, SqlBenchmarkService>();
            

            // database initialization
            services.AddHostedService<PostgresInitializerService>();
            services.AddHostedService<MongoInitializerService>();

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