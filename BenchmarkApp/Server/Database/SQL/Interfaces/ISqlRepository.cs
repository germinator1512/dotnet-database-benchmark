using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Core;

namespace BenchmarkApp.Server.Database.SQL.Interfaces
{
    public interface ISqlRepository
    {
        Task<IEnumerable<IUserEntity>> GetAllEntitiesAsync();
    }
}