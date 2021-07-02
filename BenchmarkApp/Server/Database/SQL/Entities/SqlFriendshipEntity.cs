namespace BenchmarkApp.Server.Database.SQL.Entities
{
    public class SqlFriendshipEntity
    {
        public SqlUserEntity FriendA { get; set; }

        public int FriendAId { get; set; }

        public SqlUserEntity FriendB { get; set; }

        public int FriendBId { get; set; }
    }
}