using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BenchmarkApp.Server.Database.SQL.Entities
{
    public class SqlWriteFriendshipEntity
    {
        [ForeignKey("FriendA")]
        public Guid FriendAId { get; set; }
        public SqlWriteUserEntity FriendA { get; set; }


        public Guid FriendBId { get; set; }
        public SqlWriteUserEntity FriendB { get; set; }
    }
}