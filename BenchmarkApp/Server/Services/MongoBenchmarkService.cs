using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Mongo.Interfaces;
using BenchmarkApp.Server.Services.Interfaces;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Services
{
    public class MongoBenchmarkService : IMongoBenchmarkService
    {
        private readonly IMongoRepository _mongoRepository;
        public MongoBenchmarkService(IMongoRepository mongoRepository) => _mongoRepository = mongoRepository;

        public async Task<IEnumerable<BenchmarkResult>> StartBenchmark()
        {
            var results = new List<BenchmarkResult>();
            foreach (var i in Enumerable.Range(0, 6))
            {
                results.Add(await StartBenchmarkWithLevel(i));
            }

            return results;
        }

        public async Task<BenchmarkResult> StartBenchmarkWithLevel(int level)
        {
            var timer = new Stopwatch();
            timer.Start();

            try
            {
                var entities = await _mongoRepository.GetAllFriendsAsync(level);
                timer.Stop();

                return new BenchmarkResult
                {
                    Level = level,
                    Success = true,
                    TimeSpan = timer.Elapsed,
                    LoadedEntities = Math.Pow(Config.FriendsPerUser, level + 1)
                };
            }
            catch (Exception e)
            {
                timer.Stop();
                return new BenchmarkResult
                {
                    Level = level,
                    Success = false,
                    TimeSpan = timer.Elapsed,
                    LoadedEntities = Math.Pow(Config.FriendsPerUser, level + 1)
                };
            }
        }
    }
}