using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo.Services;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

namespace BenchmarkApp.Server.Test.Unit
{
    [TestClass]
    public class MongoRepositoryTest
    {
        private readonly UnitFactory<Startup> _factory;
        private readonly MongoDatabaseContext _context;

        public MongoRepositoryTest()
        {
            _factory = new UnitFactory<Startup>();
            _context = _factory.GetRequiredService<MongoDatabaseContext>();
        }

        [TestInitialize]
        public async Task InitializeAsync()
        {
        }

        [TestMethod]
        public async Task Test()
        {
            var users = await _context.Users.Find(_ => true).ToListAsync();
            users.Count.Should().Be(0);
        }
    }
}