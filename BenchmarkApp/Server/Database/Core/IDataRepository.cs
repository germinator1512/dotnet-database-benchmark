using System.Threading.Tasks;

namespace BenchmarkApp.Server.Database.Core
{
    /// <summary>
    /// generic interface for simple extension of benchmark application
    /// </summary>
    /// <typeparam name="T">Database Repository to load data from</typeparam>
    public interface IDataRepository<T> where T : class
    {
        // read functions
        
        /// <summary>
        /// loads 10 users with 10 ^ (level+1) nested users from database // neighbors second test
        /// </summary>
        /// <param name="level"></param>
        /// <returns>total number of loaded entities</returns>
        Task<int> LoadNestedEntitiesAsync(int level);

        /// <summary>
        /// loads 10 ^ (level + 1) users from database // single-read test
        /// </summary>
        /// <param name="level"></param>
        /// <returns>total number of loaded entities</returns>
        Task<int> LoadEntitiesAsync(int level);
        
        /// <summary>
        /// loads aggregated value of 10 ^ (level + 1) users from database  // aggregation test
        /// </summary>
        /// <param name="level"></param>
        /// <returns>total number of loaded entities</returns>
        Task<int> LoadAggregateAsync(int level);
       
        
        
        // write functions
        
        /// <summary>
        /// writes 10 ^ (level + 1) users to database
        /// </summary>
        /// <param name="level"></param>
        /// <returns>total number of written entities</returns>
        Task<int> WriteEntitiesAsync(int level);


        /// <summary>
        /// writes 10 ^ (level + 1) nested users to database
        /// </summary>
        /// <param name="level"></param>
        /// <returns>total number of written entities</returns>
        Task<int> WriteNestedEntitiesAsync(int level);
        
        
        
        
        // Helper functions
        
        /// <summary>
        /// connects to database to avoid "cold start" issues
        /// </summary>
        /// <returns></returns>
        Task ConnectAsync();

        /// <summary>
        /// Truncates all Data from tables specified for read operations
        /// </summary>
        /// <returns></returns>
        Task EmptyReadDatabaseAsync();
        
        
        /// <summary>
        /// Truncates all Data from tables specified for write operations
        /// </summary>
        /// <returns></returns>
        Task EmptyWriteDatabaseAsync();

        /// <summary>
        /// Checks if database contains any data
        /// </summary>
        /// <returns></returns>
        Task<bool> IsReadDatabaseEmptyAsync();
    }
}