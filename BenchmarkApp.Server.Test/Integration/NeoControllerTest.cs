using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Neo4J.Services;
using BenchmarkApp.Shared;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace BenchmarkApp.Server.Test.Integration
{
    [TestClass]
    public class NeoControllerTest
    {
        private readonly HttpClient _client;
        private readonly Mock<IDataRepository<Neo4JRepository>> _neoRepo;
        private const int MaxLevel = 6;

        public NeoControllerTest()
        {
            var factory = new CustomWebApplicationFactory<Startup>();
            _client = factory.CreateClient();
            _neoRepo = factory.Services.GetRequiredService<Mock<IDataRepository<Neo4JRepository>>>();
        }

        [TestMethod]
        public async Task NeoUser()
        {
            const string url = "benchmark/neo4j/user";
            var response = await _client.GetAsync(url);
            _neoRepo.Verify(r => r.ConnectAsync(), Times.Once);

            foreach (var i in Enumerable.Range(0, 6))
            {
                _neoRepo.Verify(r => r.LoadEntitiesAsync(i), Times.Once);
            }

            var result = JsonConvert.DeserializeObject<BenchmarkResult[]>(
                await response.Content.ReadAsStringAsync()
            );

            result.Length.Should().Be(MaxLevel);

            foreach (var benchmarkResult in result)
            {
                benchmarkResult.Success.Should().Be(true);
            }

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }


        [TestMethod]
        public async Task NeoNeighbour()
        {
            const string url = "benchmark/neo4j/neighbour";
            var response = await _client.GetAsync(url);
            _neoRepo.Verify(r => r.ConnectAsync(), Times.Once);

            foreach (var i in Enumerable.Range(0, 6))
            {
                _neoRepo.Verify(r => r.LoadNestedEntitiesAsync(i), Times.Once);
            }

            var result = JsonConvert.DeserializeObject<BenchmarkResult[]>(
                await response.Content.ReadAsStringAsync()
            );

            result.Length.Should().Be(MaxLevel);

            foreach (var benchmarkResult in result)
            {
                benchmarkResult.Success.Should().Be(true);
            }

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task NeoAggregate()
        {
            const string url = "benchmark/neo4j/aggregate";
            var response = await _client.GetAsync(url);

            _neoRepo.Verify(r => r.ConnectAsync(), Times.Once);

            foreach (var i in Enumerable.Range(0, 6))
            {
                _neoRepo.Verify(r => r.LoadAggregateAsync(i), Times.Once);
            }

            var result = JsonConvert.DeserializeObject<BenchmarkResult[]>(
                await response.Content.ReadAsStringAsync()
            );

            result.Length.Should().Be(MaxLevel);

            foreach (var benchmarkResult in result)
            {
                benchmarkResult.Success.Should().Be(true);
            }

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task NeoWrite()
        {
            const string url = "benchmark/neo4j/write";
            var response = await _client.GetAsync(url);

            _neoRepo.Verify(r => r.ConnectAsync(), Times.Once);
            _neoRepo.Verify(r => r.EmptyWriteDatabaseAsync(), Times.Once);

            foreach (var i in Enumerable.Range(0, 6))
            {
                _neoRepo.Verify(r => r.WriteEntitiesAsync(i), Times.Once);
            }

            var result = JsonConvert.DeserializeObject<BenchmarkResult[]>(
                await response.Content.ReadAsStringAsync()
            );

            result.Length.Should().Be(MaxLevel);

            foreach (var benchmarkResult in result)
            {
                benchmarkResult.Success.Should().Be(true);
            }

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task NeoWriteNested()
        {
            const string url = "benchmark/neo4j/writeNested";
            var response = await _client.GetAsync(url);

            _neoRepo.Verify(r => r.ConnectAsync(), Times.Once);
            _neoRepo.Verify(r => r.EmptyWriteDatabaseAsync(), Times.Once);

            foreach (var i in Enumerable.Range(0, 6))
            {
                _neoRepo.Verify(r => r.WriteNestedEntitiesAsync(i), Times.Once);
            }

            var result = JsonConvert.DeserializeObject<BenchmarkResult[]>(
                await response.Content.ReadAsStringAsync()
            );

            result.Length.Should().Be(MaxLevel);

            foreach (var benchmarkResult in result)
            {
                benchmarkResult.Success.Should().Be(true);
            }

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}