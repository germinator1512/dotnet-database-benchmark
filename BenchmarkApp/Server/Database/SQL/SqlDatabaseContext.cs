using BenchmarkApp.Server.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace BenchmarkApp.Server.Database.SQL
{
    public class SqlDatabaseContext : DbContext
    {
        public SqlDatabaseContext(DbContextOptions<SqlDatabaseContext> options) : base(options)
        {
        }

        public DbSet<Entity> Entities { get; set; }
    }
}