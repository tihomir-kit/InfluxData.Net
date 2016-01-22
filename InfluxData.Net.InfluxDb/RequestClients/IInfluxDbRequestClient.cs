using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Formatters;
using InfluxData.Net.InfluxDb.Models;

namespace InfluxData.Net.InfluxDb.RequestClients
{
    public interface IInfluxDbRequestClient
    {
        /// <summary>
        /// Executes a query against the InfluxDb API in a single request. Multiple queries can be 
        /// passed in in the form of semicolon-delimited string.
        /// </summary>
        /// <param name="query">Queries to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> QueryAsync(string query);

        /// <summary>
        /// Executes a query against the database in a single request. Multiple queries can be 
        /// passed in in the form of semicolon-delimited string.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="query">Queries to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> QueryAsync(string dbName, string query);

        /// <summary>
        /// Writes series to the database based on <see cref="{WriteRequest}"/> object.
        /// </summary>
        /// <param name="dbName"><see cref="{WriteRequest}"/> object that describes the data to write.</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> PostAsync(WriteRequest writeRequest);

        IPointFormatter GetPointFormatter();

        Task<IInfluxDataApiResponse> RequestAsync(
            HttpMethod method,
            string path,
            IDictionary<string, string> requestParams = null,
            HttpContent content = null,
            bool includeAuthToQuery = true,
            bool headerIsBody = false);
    }
}