﻿using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.Neo4J.Services;
using BenchmarkApp.Server.Services.Interfaces;

namespace BenchmarkApp.Server.Services
{
    public class Neo4JBenchmarkService : BenchmarkService<Neo4JRepository>
    {
        public Neo4JBenchmarkService(IDataRepository<Neo4JRepository> repository) : base(repository)
        {
        }
    }
}