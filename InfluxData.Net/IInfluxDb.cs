using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Infrastructure.Influx;
using InfluxData.Net.Models;
using InfluxData.Net.Enums;
using InfluxData.Net.Models.Responses;
using InfluxData.Net.Infrastructure.Formatters;

namespace InfluxData.Net
{
    // NOTE: potential "regions/classes": https://docs.influxdata.com/influxdb/v0.9/query_language/

    public interface IInfluxDb
    {
        #region Database

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

        #endregion Database

        #region Basic Querying

        /// <summary>Write a single serie to the given database.</summary>
        /// <param name="dbName">The name of the database to write to.</param>
        /// <param name="point">A serie <see cref="{Point}" />.</param>
        /// <param name="retenionPolicy">The retenion policy.</param>
        /// <returns></returns>
        Task<InfluxDbApiWriteResponse> WriteAsync(string dbName, Point point, string retenionPolicy = "default");

        /// <summary>Write multiple serie points to the given database.</summary>
        /// <param name="dbName">The name of the database to write to.</param>
        /// <param name="points">A serie <see cref="Array{Point}" />.</param>
        /// <param name="retenionPolicy">The retenion policy.</param>
        /// <returns></returns>
        Task<InfluxDbApiWriteResponse> WriteAsync(string dbName, Point[] points, string retenionPolicy = "default");

        /// <summary>Execute a query agains a database.</summary>
        /// <param name="dbName">The name of the database.</param>
        /// <param name="query">The query to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <returns>A list of Series which matched the query.</returns>
        /// <exception cref="InfluxDbApiException"></exception>
        Task<List<Serie>> QueryAsync(string dbName, string query);

        #endregion Basic Querying

        #region Continuous Queries

        Task<InfluxDbApiResponse> CreateContinuousQueryAsync(ContinuousQuery continuousQuery);

        /// <summary>
        /// Describe all contious queries in a database.
        /// </summary>
        /// <param name="dbName">The name of the database for which all continuous queries should be described.</param>
        /// <returns>A list of all contious queries.</returns>
        Task<Serie> GetContinuousQueriesAsync(string dbName);

        /// <summary>
        /// Delete a continous query.
        /// </summary>
        /// <param name="dbName">The name of the database for which this query should be deleted.</param>
        /// <param name="cqName">The id of the query.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> DeleteContinuousQueryAsync(string dbName, string cqName);

        #endregion Continuous Queries

        #region Other

        /// <summary>
        /// Ping this InfluxDB.
        /// </summary>
        /// <returns>The response of the ping execution.</returns>
        Task<Pong> PingAsync();

        IFormatter GetFormatter();

        #endregion Other
    }
}