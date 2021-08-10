﻿using BenchmarkApp.Server.Database.Core;
using BenchmarkApp.Server.Database.SQL.Services;
using BenchmarkApp.Server.Services.Interfaces;

namespace BenchmarkApp.Server.Services
{
    public class SqlBenchmarkService : BenchmarkService<SqlRepository>
    {
        public SqlBenchmarkService(IDataLoader<SqlRepository> loader) : base(loader)
        {
        }
    }
}