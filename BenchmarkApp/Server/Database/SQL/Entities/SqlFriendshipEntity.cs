using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BenchmarkApp.Server.Database.SQL.Entities
{
    public class SqlFriendshipEntity
    {
        [ForeignKey("FriendA")]
        public Guid FriendAId { get; set; }
        public SqlUserEntity FriendA { get; set; }


        public Guid FriendBId { get; set; }
        public SqlUserEntity FriendB { get; set; }
    }
}