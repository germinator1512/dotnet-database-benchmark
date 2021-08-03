using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Neo4J.Interfaces;
using BenchmarkApp.Server.Services.Interfaces;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Services
{
    public class Neo4JBenchmarkService : INeo4JBenchmarkService
    {
        private readonly INeo4JRepository _neo4JRepository;
        public Neo4JBenchmarkService(INeo4JRepository neo4JRepository) => _neo4JRepository = neo4JRepository;


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
                var entities = await _neo4JRepository.GetAllFriendsAsync(level);
                timer.Stop();
                return new BenchmarkResult
                {
                    Level = level,
                    Success = true,
                    MilliSeconds = timer.ElapsedMilliseconds,
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
                    MilliSeconds = timer.ElapsedMilliseconds,
                    LoadedEntities = Math.Pow(Config.FriendsPerUser, level + 1)
                };
            }
        }
    }
}