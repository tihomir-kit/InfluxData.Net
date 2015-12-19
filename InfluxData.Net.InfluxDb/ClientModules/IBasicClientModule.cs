using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using System.Collections.Generic;
using InfluxData.Net.Common.Enums;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public interface IBasicClientModule
    {
        /// <summary>
        /// Writes a single serie point into a database.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="point">A serie <see cref="{Point}" />.</param>
        /// <param name="retenionPolicy">The retenion policy.</param>
        /// <param name="precision">InfluxDb time precision to use (defaults to 'ms')</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> WriteAsync(string dbName, Point point, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds);

        /// <summary>Writes multiple serie points to a database.</summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="points">A serie <see cref="Array{Point}" />.</param>
        /// <param name="retenionPolicy">The retenion policy.</param>
        /// <param name="precision">InfluxDb time precision to use (defaults to 'ms')</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> WriteAsync(string dbName, Point[] points, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds);

        /// <summary>Execute a query agains the database.</summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="query">The query to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <returns>A list of Series that match the query.</returns>
        /// <exception cref="InfluxDbApiException"></exception>
        Task<List<Serie>> QueryAsync(string dbName, string query);

        /// <summary>
        /// Ping this InfluxDB.
        /// </summary>
        /// <returns>Response of the ping execution (success, dbVersion, response time).</returns>
        Task<Pong> PingAsync();
    }
}