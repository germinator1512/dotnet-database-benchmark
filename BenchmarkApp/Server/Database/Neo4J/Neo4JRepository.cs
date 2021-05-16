using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.SQL.Entities;

namespace BenchmarkApp.Server.Database.Neo4J
{
    public class Neo4JRepository : INeo4JRepository
    {
        private readonly Neo4JDatabaseContext _ctx;

        public Neo4JRepository(Neo4JDatabaseContext context) => _ctx = context;

        public async Task<IEnumerable<UserEntity>> GetAllEntitiesAsync()
        {
            var query = "MATCH (entity:Entity) RETURN entity";


            List<string> result = null;
            var session = _ctx.Driver.AsyncSession();

            try
            {
                result = await session.ReadTransactionAsync(async tx =>
                {
                    var products = new List<string>();

                    var reader = await tx.RunAsync(query);

                    while (await reader.FetchAsync())
                    {
                        products.Add(reader.Current[0].ToString());
                    }

                    return products;
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

            return new List<UserEntity>();
        }
    }
}