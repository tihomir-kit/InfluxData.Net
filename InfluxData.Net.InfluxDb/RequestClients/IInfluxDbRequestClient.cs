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
        IConfiguration Configuration { get; }

        /// <summary>
        /// Executes a GET query against the InfluxDb API in a single request. Multiple queries can be 
        /// passed in in the form of semicolon-delimited string. If chunkSize is specified, responses 
        /// will be broken down by number of returned rows. 
        /// </summary>
        /// <param name="query">Queries to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <param name="dbName">Database name. (OPTIONAL)</param>
        /// <param name="epochFormat">Epoch timestamp format. (OPTIONAL)</param>
        /// <param name="chunkSize">Maximum number of rows per chunk. (OPTIONAL)</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> GetQueryAsync(string query, string dbName = null, string epochFormat = null, long? chunkSize = null);

        /// <summary>
        /// Executes a POST query against the InfluxDb API in a single request. Multiple queries can be 
        /// passed in in the form of semicolon-delimited string.
        /// </summary>
        /// <param name="query">Queries to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <param name="dbName">Database name.</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> PostQueryAsync(string query, string dbName = null);

        /// <summary>
        /// Writes series to the database based on <see cref="{WriteRequest}"/> object.
        /// </summary>
        /// <param name="dbName"><see cref="{WriteRequest}"/> object that describes the data to write.</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> PostAsync(WriteRequest writeRequest);

        /// <summary>
        /// Executes a query against the InfluxDb API in a single request. Multiple queries can be 
        /// passed in in the form of semicolon-delimited string. If chunkSize is specified, responses 
        /// will be broken down by number of returned rows. 
        /// </summary>
        /// <param name="query">Queries to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <param name="method">The HTTP method to use when executing the query.</param>
        /// <param name="dbName">Database name. (OPTIONAL)</param>
        /// /// <param name="epochFormat">Epoch timestamp format. (OPTIONAL)</param>
        /// <param name="chunkSize">Maximum number of rows per chunk. (OPTIONAL)</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> QueryAsync(string query, HttpMethod method, string dbName = null, string epochFormat = null, long? chunkSize = null);

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