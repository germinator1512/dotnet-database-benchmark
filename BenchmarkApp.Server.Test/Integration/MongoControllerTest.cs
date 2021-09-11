using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Mongo.Services;
using BenchmarkApp.Shared;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;

namespace BenchmarkApp.Server.Test.Integration
{
    [TestClass]
    public class MongoControllerTest
    {
        private readonly HttpClient _client;
        private readonly Mock<IDataRepository<MongoRepository>> _mongoRepo;
        private const int MaxLevel = 6;

        public MongoControllerTest()
        {
            var factory = new CustomWebApplicationFactory<Startup>();

            _client = factory.CreateClient();
            _mongoRepo = factory.Services.GetRequiredService<Mock<IDataRepository<MongoRepository>>>();
        }

        [TestMethod]
        public async Task MongoUser()
        {
            const string url = "benchmark/mongo/user";
            var response = await _client.GetAsync(url);
            _mongoRepo.Verify(r => r.ConnectAsync(), Times.Once);

            foreach (var i in Enumerable.Range(0, MaxLevel))
            {
                _mongoRepo.Verify(r => r.LoadEntitiesAsync(i), Times.Once);
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
        public async Task MongoNeighbour()
        {
            const string url = "benchmark/mongo/neighbour";
            var response = await _client.GetAsync(url);
            _mongoRepo.Verify(r => r.ConnectAsync(), Times.Once);

            foreach (var i in Enumerable.Range(0, 6))
            {
                _mongoRepo.Verify(r => r.LoadNestedEntitiesAsync(i), Times.Once);
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
        public async Task MongoAggregate()
        {
            const string url = "benchmark/mongo/aggregate";
            var response = await _client.GetAsync(url);

            _mongoRepo.Verify(r => r.ConnectAsync(), Times.Once);

            foreach (var i in Enumerable.Range(0, 6))
            {
                _mongoRepo.Verify(r => r.LoadAggregateAsync(i), Times.Once);
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
        public async Task MongoWrite()
        {
            const string url = "benchmark/mongo/write";
            var response = await _client.GetAsync(url);

            _mongoRepo.Verify(r => r.ConnectAsync(), Times.Once);
            _mongoRepo.Verify(r => r.EmptyWriteDatabaseAsync(), Times.Once);

            foreach (var i in Enumerable.Range(0, 6))
            {
                _mongoRepo.Verify(r => r.WriteEntitiesAsync(i), Times.Once);
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
        public async Task MongoWriteNested()
        {
            const string url = "benchmark/mongo/writeNested";
            var response = await _client.GetAsync(url);

            _mongoRepo.Verify(r => r.ConnectAsync(), Times.Once);
            _mongoRepo.Verify(r => r.EmptyWriteDatabaseAsync(), Times.Once);

            foreach (var i in Enumerable.Range(0, 6))
            {
                _mongoRepo.Verify(r => r.WriteNestedEntitiesAsync(i), Times.Once);
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