using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Database.Core
{
    public class TimerService
    {
        /// <summary>
        /// measures execution time of the given function
        /// </summary>
        /// <param name="timerFunction">function to execute</param>
        /// <param name="level">parameter for timerFunction</param>
        /// <returns></returns>
        public static async Task<BenchmarkResult> ExecuteBenchmarkAsync(
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