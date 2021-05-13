using System.Threading.Tasks;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Services.Interfaces
{
    public interface IBenchmarkService
    {
        Task<BenchmarkResult> StartBenchmark();
    }
}