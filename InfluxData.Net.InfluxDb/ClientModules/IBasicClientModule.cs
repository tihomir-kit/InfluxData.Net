using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using System.Collections.Generic;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public interface IBasicClientModule
    {
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

        /// <summary>
        /// Ping this InfluxDB.
        /// </summary>
        /// <returns>The response of the ping execution.</returns>
        Task<Pong> PingAsync();
    }
}