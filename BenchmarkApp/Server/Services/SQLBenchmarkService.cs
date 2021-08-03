using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.SQL.Interfaces;
using BenchmarkApp.Server.Services.Interfaces;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Services
{
    public class SqlBenchmarkService : ISQLBenchmarkService
    {
        private readonly ISqlRepository _sqlRepository;
        public SqlBenchmarkService(ISqlRepository sqlRepository) => _sqlRepository = sqlRepository;

        public async Task<IEnumerable<BenchmarkResult>> StartFriendsWithNeighboursBenchmarkAsync()
        {
            var results = new List<BenchmarkResult>();
            foreach (var i in Enumerable.Range(0, 6))
            {
                var result = await StartNeighbourBenchmarkWithLevel(i);
                results.Add(result);
            }

            return results;
        }

        public async Task<IEnumerable<BenchmarkResult>> StartUserBenchmarkAsync()
        {
            await _sqlRepository.ConnectAsync();
            
            var results = new List<BenchmarkResult>();
            foreach (var i in Enumerable.Range(0, 6))
            {
                var result = await StartUserBenchmarkWithLevel(i);
                results.Add(result);
            }

            return results;
        }


        private async Task<BenchmarkResult> StartUserBenchmarkWithLevel(int level)
        {
            var timer = new Stopwatch();
            var numberToLoad = Math.Pow(Config.FriendsPerUser, level + 1);

            timer.Start();
            try
            {
                var entities = await _sqlRepository.GetUserAsync((int) numberToLoad);

                timer.Stop();
                return new BenchmarkResult
                {
                    Level = level,
                    Success = true,
                    MilliSeconds = timer.ElapsedMilliseconds,
                    LoadedEntities = numberToLoad
                };
            }
            catch (Exception e)
            {
                timer.Stop();
                return new BenchmarkResult
                {
                    Level = level,
                    Success = false,
                    MilliSeconds = timer.ElapsedMilliseconds,
                    LoadedEntities = numberToLoad
                };
            }
        }


        private async Task<BenchmarkResult> StartNeighbourBenchmarkWithLevel(int level)
        {
            var timer = new Stopwatch();
            timer.Start();
            var numberToLoad = Math.Pow(Config.FriendsPerUser, level + 1);
            try
            {
                var entities = await _sqlRepository.GetAllFriendsAsync(level);

                timer.Stop();
                return new BenchmarkResult
                {
                    Level = level,
                    Success = true,
                    MilliSeconds = timer.ElapsedMilliseconds,
                    LoadedEntities = numberToLoad
                };
            }
            catch (Exception e)
            {
                timer.Stop();
                return new BenchmarkResult
                {
                    Level = level,
                    Success = false,
                    MilliSeconds = timer.ElapsedMilliseconds,
                    LoadedEntities = numberToLoad
                };
            }
        }
    }
}