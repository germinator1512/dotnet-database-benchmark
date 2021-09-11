using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.SQL.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace BenchmarkApp.Server.Test.Integration
{
    [TestClass]
    public class SqlControllerTest
    {
        private readonly HttpClient _client;
        private readonly Mock<IDataRepository<SqlRepository>> _sqlRepo;
        private const int MaxLevel = 6;


        public SqlControllerTest()
        {
            var factory = new CustomWebApplicationFactory<Startup>();
            _client = factory.CreateClient();
            _sqlRepo = factory.Services.GetRequiredService<Mock<IDataRepository<SqlRepository>>>();
        }

        [TestMethod]
        public async Task SqlUser()
        {
            const string url = "benchmark/sql/user";
            var response = await _client.GetAsync(url);
            _sqlRepo.Verify(r => r.ConnectAsync(), Times.Once);

            foreach (var i in Enumerable.Range(0, MaxLevel))
            {
                _sqlRepo.Verify(r => r.LoadEntitiesAsync(i), Times.Once);
            }

            await response.VerifyBenchmarkResult(MaxLevel);
        }


        [TestMethod]
        public async Task SqlNeighbour()
        {
            const string url = "benchmark/sql/neighbour";
            var response = await _client.GetAsync(url);
            _sqlRepo.Verify(r => r.ConnectAsync(), Times.Once);

            foreach (var i in Enumerable.Range(0, MaxLevel))
            {
                _sqlRepo.Verify(r => r.LoadNestedEntitiesAsync(i), Times.Once);
            }

            await response.VerifyBenchmarkResult(MaxLevel);
        }

        [TestMethod]
        public async Task SqlAggregate()
        {
            const string url = "benchmark/sql/aggregate";
            var response = await _client.GetAsync(url);

            _sqlRepo.Verify(r => r.ConnectAsync(), Times.Once);

            foreach (var i in Enumerable.Range(0, MaxLevel))
            {
                _sqlRepo.Verify(r => r.LoadAggregateAsync(i), Times.Once);
            }

            await response.VerifyBenchmarkResult(MaxLevel);
        }

        [TestMethod]
        public async Task SqlWrite()
        {
            const string url = "benchmark/sql/write";
            var response = await _client.GetAsync(url);

            _sqlRepo.Verify(r => r.ConnectAsync(), Times.Once);
            _sqlRepo.Verify(r => r.EmptyWriteDatabaseAsync(), Times.Once);

            foreach (var i in Enumerable.Range(0, MaxLevel))
            {
                _sqlRepo.Verify(r => r.WriteEntitiesAsync(i), Times.Once);
            }

            await response.VerifyBenchmarkResult(MaxLevel);
        }

        [TestMethod]
        public async Task SqlWriteNested()
        {
            const string url = "benchmark/sql/writeNested";
            var response = await _client.GetAsync(url);

            _sqlRepo.Verify(r => r.ConnectAsync(), Times.Once);
            _sqlRepo.Verify(r => r.EmptyWriteDatabaseAsync(), Times.Once);

            foreach (var i in Enumerable.Range(0, MaxLevel))
            {
                _sqlRepo.Verify(r => r.WriteNestedEntitiesAsync(i), Times.Once);
            }

            await response.VerifyBenchmarkResult(MaxLevel);
        }
    }
}