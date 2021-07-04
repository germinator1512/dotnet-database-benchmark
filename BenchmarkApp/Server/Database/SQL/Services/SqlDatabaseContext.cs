using BenchmarkApp.Server.Database.SQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BenchmarkApp.Server.Database.SQL.Services
{
    public class SqlDatabaseContext : DbContext
    {
        public SqlDatabaseContext(DbContextOptions<SqlDatabaseContext> options) : base(options)
        {
        }

        public DbSet<SqlUserEntity> Users { get; set; }

        public DbSet<SqlFriendshipEntity> Friendships { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SqlUserEntity>()
                .HasMany(f => f.FriendShips);

            modelBuilder.Entity<SqlFriendshipEntity>()
                .HasKey(f => new {f.FriendAId, f.FriendBId});

            modelBuilder.Entity<SqlFriendshipEntity>()
                .HasOne(g => g.FriendA);


            modelBuilder.Entity<SqlFriendshipEntity>()
                .HasOne(g => g.FriendB);
        }
    }
}