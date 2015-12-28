using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using System;

namespace InfluxData.Net.InfluxDb.RequestClients.Modules
{
    public interface IDatabaseRequestModule
    {
        /// <summary>
        /// Creates a new Database.
        /// </summary>
        /// <param name="dbName">The name of the new database</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> CreateDatabase(string dbName);

        /// <summary>
        /// Gets all available databases.
        /// </summary>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> GetDatabases();

        /// <summary>
        /// Drops a database.
        /// </summary>
        /// <param name="dbName">The name of the database to delete.</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> DropDatabase(string dbName);

        /// <summary>
        /// Deletes all data points from a serie.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="serieName">Serie name.</param>
        /// <returns></returns>
        [Obsolete("Plese use 'DropSeries' from .Serie instead.")]
        Task<IInfluxDbApiResponse> DropSeries(string dbName, string serieName);
    }
}