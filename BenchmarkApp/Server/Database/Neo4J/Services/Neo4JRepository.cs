﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Neo4J.Entities;
using BenchmarkApp.Server.Database.Neo4J.Interfaces;

namespace BenchmarkApp.Server.Database.Neo4J.Services
{
    public class Neo4JRepository : INeo4JRepository
    {
        private readonly Neo4JDatabaseContext _ctx;

        public Neo4JRepository(Neo4JDatabaseContext context) => _ctx = context;

        public async Task<IEnumerable<Neo4jUserEntity>> GetAllEntitiesAsync()
        {
            var query = "MATCH (user:User) RETURN user";

            List<string> result = null;
            var session = _ctx.Driver.AsyncSession();

            try 
            {
                result = await session.ReadTransactionAsync(async tx =>
                {
                    var users = new List<string>();

                    var reader = await tx.RunAsync(query);

                    while (await reader.FetchAsync())
                    {
                        users.Add(reader.Current[0].ToString());
                    }

                    return users;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                await session.CloseAsync();
            }

            return new List<Neo4jUserEntity>();
        }

        public Task AddEntitiesAsync(IEnumerable<Neo4jUserEntity> users)
        {
            return null;
        }
    }
}