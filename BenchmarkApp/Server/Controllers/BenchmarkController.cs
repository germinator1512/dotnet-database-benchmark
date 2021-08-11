﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Mongo.Services;
using BenchmarkApp.Server.Database.Neo4J.Services;
using BenchmarkApp.Server.Database.SQL.Services;
using BenchmarkApp.Server.Services.Interfaces;
using BenchmarkApp.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BenchmarkApp.Server.Controllers
{
    [ApiController]
    [Route("benchmark")]
    public class BenchmarkController : ControllerBase
    {
        private readonly ILogger<BenchmarkController> _logger;
        private readonly BenchmarkService<MongoRepository> _mongoBenchmarkService;
        private readonly BenchmarkService<SqlRepository> _sqlBenchmarkService;
        private readonly BenchmarkService<Neo4JRepository> _neo4JBenchmarkService;

        public BenchmarkController(ILogger<BenchmarkController> logger,
            BenchmarkService<MongoRepository> mongoBenchmarkService,
            BenchmarkService<SqlRepository> sqlBenchmarkService,
            BenchmarkService<Neo4JRepository> neo4JBenchmarkService)
        {
            _logger = logger;
            _mongoBenchmarkService = mongoBenchmarkService;
            _sqlBenchmarkService = sqlBenchmarkService;
            _neo4JBenchmarkService = neo4JBenchmarkService;
        }

        [HttpGet("mongo/neighbour")]
        public async Task<IEnumerable<BenchmarkResult>> MongoNeighbour()
        {
            _logger.Log(LogLevel.Debug, "Starting Mongo Benchmark");
            return await _mongoBenchmarkService.StartFriendsWithNeighboursBenchmarkAsync();
        }

        [HttpGet("mongo/user")]
        public async Task<IEnumerable<BenchmarkResult>> MongoUser()
        {
            _logger.Log(LogLevel.Debug, "Starting Mongo Benchmark");
            return await _mongoBenchmarkService.StartUserBenchmarkAsync();
        }

        [HttpGet("mongo/aggregate")]
        public async Task<IEnumerable<BenchmarkResult>> MongoAggregate()
        {
            _logger.Log(LogLevel.Debug, "Starting Mongo Benchmark");
            return await _mongoBenchmarkService.StartAggregateBenchmarkAsync();
        }


        [HttpGet("sql/neighbour")]
        public async Task<IEnumerable<BenchmarkResult>> SqlNeighbour()
        {
            _logger.Log(LogLevel.Debug, "Starting SQL Benchmark");
            return await _sqlBenchmarkService.StartFriendsWithNeighboursBenchmarkAsync();
        }

        [HttpGet("sql/user")]
        public async Task<IEnumerable<BenchmarkResult>> SqlUser()
        {
            _logger.Log(LogLevel.Debug, "Starting SQL Benchmark");
            return await _sqlBenchmarkService.StartUserBenchmarkAsync();
        }

        [HttpGet("sql/aggregate")]
        public async Task<IEnumerable<BenchmarkResult>> SqlAggregate()
        {
            _logger.Log(LogLevel.Debug, "Starting SQL Benchmark");
            return await _sqlBenchmarkService.StartAggregateBenchmarkAsync();
        }


        [HttpGet("neo4j/neighbour")]
        public async Task<IEnumerable<BenchmarkResult>> Neo4JNeighbour()
        {
            _logger.Log(LogLevel.Debug, "Starting Neo4J Benchmark");
            return await _neo4JBenchmarkService.StartFriendsWithNeighboursBenchmarkAsync();
        }

        [HttpGet("neo4j/user")]
        public async Task<IEnumerable<BenchmarkResult>> Neo4JUser()
        {
            _logger.Log(LogLevel.Debug, "Starting Neo4J Benchmark");
            return await _neo4JBenchmarkService.StartUserBenchmarkAsync();
        }

        [HttpGet("neo4j/aggregate")]
        public async Task<IEnumerable<BenchmarkResult>> Neo4Aggregate()
        {
            _logger.Log(LogLevel.Debug, "Starting Neo4J Benchmark");
            return await _neo4JBenchmarkService.StartAggregateBenchmarkAsync();
        }
    }
}