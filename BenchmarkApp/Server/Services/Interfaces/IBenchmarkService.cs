﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Shared;

namespace BenchmarkApp.Server.Services.Interfaces
{
    public interface IBenchmarkService<T>
    {
        Task<IEnumerable<BenchmarkResult>> StartFriendsWithNeighboursBenchmarkAsync();

        Task<IEnumerable<BenchmarkResult>> StartUserBenchmarkAsync();
    }
}