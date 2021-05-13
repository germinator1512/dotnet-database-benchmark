using System.Collections.Generic;
using System.Threading.Tasks;
using BenchmarkApp.Server.Database.Entities;

namespace BenchmarkApp.Server.Database.Neo4J
{
    public class Neo4JRepository : INeo4JRepository
    {
        private readonly Neo4JDatabaseContext _ctx;

        public Neo4JRepository(Neo4JDatabaseContext context) => _ctx = context;

        public async Task<IEnumerable<Entity>> GetAllEntitiesAsync()
        {
            var query = "MATCH (entity:Entity) RETURN entity";


            List<string> result = null;
            var session = _ctx.Driver.AsyncSession();

            try
            {
                // Wrap whole operation into an managed transaction and
                // get the results back.
                result = await session.ReadTransactionAsync(async tx =>
                {
                    var products = new List<string>();

                    // Send cypher query to the database
                    var reader = await tx.RunAsync(query);

                    // Loop through the records asynchronously
                    while (await reader.FetchAsync())
                    {
                        // Each current read in buffer can be reached via Current
                        products.Add(reader.Current[0].ToString());
                    }

                    return products;
                });
            }
            finally
            {
                // asynchronously close session
                await session.CloseAsync();
            }

            return new List<Entity>();
        }
    }
}