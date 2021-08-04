using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Database.Core
{
    public class TimerService
    {
        public static async Task<IEnumerable<BenchmarkResult>> Benchmark<T>(IDataLoader<T> loader,
            Func<int, Task<int>> benchmarkFunction)
        {
            await loader.ConnectAsync();

            var results = new List<BenchmarkResult>();
            foreach (var i in Enumerable.Range(0, Config.NestedUserLevels))
            {
                var result = await GetExecutionTime(benchmarkFunction, i);
                results.Add(result);
            }

            return results;
        }

        private static async Task<BenchmarkResult> GetExecutionTime(
            Func<int, Task<int>> timerFunction,
            int level)
        {
            var timer = new Stopwatch();
            timer.Start();

            try
            {
                var loadedEntities = await timerFunction(level);

                timer.Stop();
                return new BenchmarkResult
                {
                    Level = level,
                    Success = true,
                    LoadedEntities = loadedEntities,
                    MilliSeconds = timer.ElapsedMilliseconds
                };
            }
            catch (Exception e)
            {
                timer.Stop();
                return new BenchmarkResult
                {
                    Level = level,
                    Success = false,
                    LoadedEntities = 0,
                    MilliSeconds = timer.ElapsedMilliseconds,
                    Error = e.Message
                };
            }
        }
    }
}