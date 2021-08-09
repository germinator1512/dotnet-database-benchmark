using System.Threading.Tasks;

namespace BenchmarkApp.Server.Database.Core
{
    public interface IDataLoader<T>
    {
        /// <summary>
        /// loads 10 users with 10 ^ (level+1) nested users from database
        /// </summary>
        /// <param name="level"></param>
        /// <returns>total number of loaded entities</returns>
        Task<int> LoadNestedEntitiesAsync(int level);

        /// <summary>
        /// loads 10 ^ (level + 1) users from database
        /// </summary>
        /// <param name="level"></param>
        /// <returns>total number of loaded entities</returns>
        Task<int> LoadEntitiesAsync(int level);

        /// <summary>
        /// connects to database to avoid "cold start" issues
        /// </summary>
        /// <returns></returns>
        Task ConnectAsync();

        /// <summary>
        /// Truncates all Data from Database
        /// </summary>
        /// <returns></returns>
        Task EmptyDatabaseAsync();

        /// <summary>
        /// Checks if database contains any data
        /// </summary>
        /// <returns></returns>
        Task<bool> IsDatabaseEmptyAsync();
    }
}