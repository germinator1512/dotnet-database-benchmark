using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.SQL.Entities;
using BenchmarkApp.Server.Database.SQL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BenchmarkApp.Server.Database.SQL.Services
{
    public class SqlRepository : ISqlRepository
    {
        private readonly SqlDatabaseContext _ctx;

        public SqlRepository(SqlDatabaseContext context) => _ctx = context;

        public async Task<IEnumerable<SqlUserEntity>> GetAllEntitiesAsync()
        {
            return await _ctx.Users.ToListAsync();
        }

        public async Task AddEntitiesAsync(IEnumerable<SqlUserEntity> users)
        {
            await _ctx.Users.AddRangeAsync(users);
            await _ctx.SaveChangesAsync();
        }
    }
}