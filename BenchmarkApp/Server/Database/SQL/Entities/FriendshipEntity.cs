namespace BenchmarkApp.Server.Database.SQL.Entities
{
    public class FriendshipEntity
    {
        public UserEntity FriendA { get; set; }

        public int FriendAId { get; set; }

        public UserEntity FriendB { get; set; }

        public int FriendBId { get; set; }
    }
}