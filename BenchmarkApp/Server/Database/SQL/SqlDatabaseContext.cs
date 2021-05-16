using BenchmarkApp.Server.Database.SQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BenchmarkApp.Server.Database.SQL
{
    public class SqlDatabaseContext : DbContext
    {
        public SqlDatabaseContext(DbContextOptions<SqlDatabaseContext> options) : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }

        public DbSet<FriendshipEntity> Friendships { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>()
                .HasMany(u => u.Friends);


            modelBuilder.Entity<FriendshipEntity>()
                .HasKey(f => new {f.FriendAId, f.FriendBId});


            modelBuilder.Entity<FriendshipEntity>()
                .HasOne(g => g.FriendA);


            modelBuilder.Entity<FriendshipEntity>()
                .HasOne(g => g.FriendB);
        }
    }
}