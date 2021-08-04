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
        /// <summary>
        /// executes the given function with 0 .. Config.NestedUsers as parameters
        /// </summary>
        /// <param name="loader">repository to connect to</param>
        /// <param name="benchmarkFunction">function to execute </param>
        /// <typeparam name="T"></typeparam>
        /// <returns>List of Benchmark-results of given function</returns>
        public static async Task<IEnumerable<BenchmarkResult>> BenchmarkAsync<T>(
            IDataLoader<T> loader,
            Func<int, Task<int>> benchmarkFunction)
        {
            await loader.ConnectAsync();

            var results = new List<BenchmarkResult>();
            foreach (var level in Enumerable.Range(0, Config.NestedUserLevels))
            {
                var result = await ExecuteBenchmarkAsync(benchmarkFunction, level);
                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// measures execution time of the given function
        /// </summary>
        /// <param name="timerFunction">function to execute</param>
        /// <param name="level">parameter for timerFunction</param>
        /// <returns></returns>
        private static async Task<BenchmarkResult> ExecuteBenchmarkAsync(
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