namespace BenchmarkApp.Server.Database.Core
{
    public static class Config
    {
        public static string UserName(int level, int number) => $"Level {level} Friend {number}";

        public const string RootUserName = "Root-User";
        
        // standard is 6
        public const int NestedUserLevels = 6;
        public const int FriendsPerUser = 10;
    }
}