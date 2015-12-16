using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using System.Collections.Generic;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public interface IDatabaseClientModule
    {
        /// <summary>
        /// Create a new Database.
        /// </summary>
        /// <param name="dbName">The name of the new database</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> CreateDatabaseAsync(string dbName);

        /// <summary>
        /// Drop a database.
        /// </summary>
        /// <param name="dbName">The name of the database to delete.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> DropDatabaseAsync(string dbName);

        /// <summary>
        /// Describe all available databases.
        /// </summary>
        /// <returns>A list of all Databases</returns>
        Task<List<DatabaseResponse>> ShowDatabasesAsync();

        /// <summary>
        /// Delete a serie.
        /// </summary>
        /// <param name="dbName">The database in which the given serie should be deleted.</param>
        /// <param name="serieName">The name of the serie.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> DropSeriesAsync(string dbName, string serieName);

        Task<InfluxDbApiResponse> AlterRetentionPolicy(string policyName, string dbName, string duration, int replication);
    }
}