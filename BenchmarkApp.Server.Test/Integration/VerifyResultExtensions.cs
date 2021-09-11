using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BenchmarkApp.Shared;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace BenchmarkApp.Server.Test.Integration
{
    public static class VerifyResultExtensions
    {
        public static async Task VerifyBenchmarkResult(this HttpResponseMessage response, int howMany)
        {
            var result = JsonConvert.DeserializeObject<BenchmarkResult[]>(
                await response.Content.ReadAsStringAsync()
            );

            result.Length.Should().Be(howMany);

            foreach (var benchmarkResult in result)
            {
                benchmarkResult.Success.Should().Be(true);
            }
            
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
    }
}