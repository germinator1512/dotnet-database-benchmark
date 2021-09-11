using System;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.SQL.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BenchmarkApp.Server.Test.Unit
{
    [TestClass]
    public class SqlRepositoryTest
    {
        private readonly UnitFactory<Startup> _factory;
        private readonly SqlDatabaseContext _context;
        private readonly IDataRepository<SqlRepository> _repository;

        public SqlRepositoryTest()
        {
            _factory = new UnitFactory<Startup>();
            _context = _factory.GetRequiredService<SqlDatabaseContext>();
            _repository = _factory.GetRequiredService<IDataRepository<SqlRepository>>();
        }

        [TestInitialize]
        public async Task InitializeAsync() => await _context.Database.MigrateAsync();


        [TestMethod]
        public async Task Users()
        {
            var loadedEntities = await _repository.LoadEntitiesAsync(2);
            loadedEntities.Should().Be((int) Math.Pow(10, 3));
        }

        [TestMethod]
        public async Task Neighbour()
        {
            var loadedEntities = await _repository.LoadNestedEntitiesAsync(2);
            loadedEntities.Should().Be((int) Math.Pow(10, 3));
        }

        [TestMethod]
        public async Task Aggregate()
        {
            var loadedEntities = await _repository.LoadAggregateAsync(2);
            loadedEntities.Should().Be((int) Math.Pow(10, 3));
        }

        [TestMethod]
        public async Task Write()
        {
            var loadedEntities = await _repository.WriteEntitiesAsync(2);
            loadedEntities.Should().Be((int) Math.Pow(10, 3));
        }

        [TestMethod]
        public async Task WriteNested()
        {
            var loadedEntities = await _repository.WriteNestedEntitiesAsync(2);
            loadedEntities.Should().Be((int) Math.Pow(10, 3));
        }
    }
}