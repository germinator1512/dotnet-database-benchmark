using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Neo4jClient;

namespace BenchmarkApp.Server.Test.Unit
{
    [TestClass]
    public class NeoRepositoryTest
    {
        private readonly UnitFactory<Startup> _factory;
        private readonly IGraphClient _client;

        public NeoRepositoryTest()
        {
            _factory = new UnitFactory<Startup>();
            _client = _factory.GetRequiredService<IGraphClient>();
        }

        [TestInitialize]
        public async Task InitializeAsync() => await _client.ConnectAsync();
        

        [TestMethod]
        public async Task Test()
        {
            
            var empty  = (await _client.Cypher
                    .Match("(n:User)")
                    .Return(n => n.Count())
                    .Limit(1)
                    .ResultsAsync)
                .Single() == 0;
            
            empty.Should().Be(true);
        }
    }
}