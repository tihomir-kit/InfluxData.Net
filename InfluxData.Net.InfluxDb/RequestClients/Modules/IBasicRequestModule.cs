using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using System.Threading.Tasks;

namespace InfluxData.Net.InfluxDb.RequestClients.Modules
{
    public interface IBasicRequestModule
    {
        /// <summary>
        /// Writes series to the database based on <see cref="{WriteRequest}"/> object.
        /// </summary>
        /// <param name="dbName"><see cref="{WriteRequest}"/> object that describes the data to write.</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> Write(WriteRequest writeRequest);

        /// <summary>
        /// Executes a query agains the database.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="query">Query to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <returns>A list of Series that match the query.</returns>
        Task<IInfluxDbApiResponse> Query(string dbName, string query);
    }
}