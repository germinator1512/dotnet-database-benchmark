namespace BenchmarkApp.Shared
{
    public class BenchmarkResult
    {
        public bool Success { get; set; }

        public int Level { get; set; }

        public long MilliSeconds { get; set; }

        public double LoadedEntities { get; set; }

        public string? Error { get; set; }
    }
}