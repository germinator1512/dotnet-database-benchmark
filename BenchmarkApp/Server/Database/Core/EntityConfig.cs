namespace BenchmarkApp.Server.Database.Core
{
    public static class EntityConfig
    {
        public static string UserName(int level, int number) => $"Level {level} Friend {number}";
        public const string RootUserName = "Max Mustermann";
        public const int NestedUserLevels = 6;
    }
}