using System;

namespace BenchmarkApp.Shared
{
    public class BenchmarkResult
    {
        public bool Success { get; set; }

        public int Level { get; set; }

        public TimeSpan TimeSpan { get; set; }
        
        public double LoadedEntities { get; set; }
    }
}