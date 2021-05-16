using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.SQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace BenchmarkApp.Server.Database.SQL
{
    public class SqlRepository : ISqlRepository
    {
        private readonly SqlDatabaseContext _ctx;

        public SqlRepository(SqlDatabaseContext context) => _ctx = context;

        public async Task<IEnumerable<UserEntity>> GetAllEntitiesAsync()
        {
            return await _ctx.Users.ToListAsync();
        }
    }
}