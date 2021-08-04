using System.Threading.Tasks;

namespace BenchmarkApp.Server.Database
{
    public interface IDataLoader<T>
    {
        /// <summary>
        /// loads 10 ^ level nested users from database
        /// </summary>
        /// <param name="level"></param>
        /// <returns>total number of loaded entities</returns>
        Task<int> GetAllFriendsAsync(int level);

        /// <summary>
        /// loads 10 ^ level users from database
        /// </summary>
        /// <param name="level"></param>
        /// <returns>total number of loaded entities</returns>
        Task<int> GetUserAsync(int level);

        /// <summary>
        /// connects to database to avoid "cold start" issues
        /// </summary>
        /// <returns></returns>
        Task ConnectAsync();

        /// <summary>
        /// Truncates all Data from Database
        /// </summary>
        /// <returns></returns>
        Task EmptyDatabase();

        /// <summary>
        /// Checks if database contains any data
        /// </summary>
        /// <returns></returns>
        Task<bool> IsDatabaseEmpty();
    }
}