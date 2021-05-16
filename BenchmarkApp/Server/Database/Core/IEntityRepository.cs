﻿using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.SQL.Entities;

namespace BenchmarkApp.Server.Database.Core
{
    public interface IEntityRepository
    {
        Task<IEnumerable<UserEntity>> GetAllEntitiesAsync();
    }
}