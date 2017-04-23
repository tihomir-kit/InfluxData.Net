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
        /// passed in in the form of semicolon-delimited string.
        /// </summary>
        /// <param name="query">Queries to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> GetQueryAsync(string query);

        /// <summary>
        /// Executes a GET query against the database in a single request. Multiple queries can be 
        /// passed in in the form of semicolon-delimited string.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="query">Queries to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> GetQueryAsync(string dbName, string query);

        /// <summary>
        /// Executes a GET query against the InfluxDb API in a single request with chunked response. Multiple queries can be 
        /// passed in in the form of semicolon-delimited string.
        /// </summary>
        /// <param name="query">Queries to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <param name="chunkSize">Maximum number of rows in each chunk</param>/// <returns></returns>
        Task<IInfluxDataApiResponse> GetQueryChunkedAsync(string query, long chunkSize);

        /// <summary>
        /// Executes a GET query against the database in a single request. Multiple queries can be 
        /// passed in in the form of semicolon-delimited string.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="query">Queries to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <param name="chunkSize">Maximum number of rows in each chunk</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> GetQueryChunkedAsync(string dbName, string query, long chunkSize);

        /// <summary>
        /// Executes a POST query against the InfluxDb API in a single request. Multiple queries can be 
        /// passed in in the form of semicolon-delimited string.
        /// </summary>
        /// <param name="query">Queries to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> PostQueryAsync(string query);

        /// <summary>
        /// Executes a POST query against the database in a single request. Multiple queries can be 
        /// passed in in the form of semicolon-delimited string.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="query">Queries to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> PostQueryAsync(string dbName, string query);

        /// <summary>
        /// Writes series to the database based on <see cref="{WriteRequest}"/> object.
        /// </summary>
        /// <param name="dbName"><see cref="{WriteRequest}"/> object that describes the data to write.</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> PostAsync(WriteRequest writeRequest);

        /// <summary>
        /// Executes a query against the InfluxDb API in a single request. Multiple queries can be 
        /// passed in in the form of semicolon-delimited string.
        /// </summary>
        /// <param name="query">Queries to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <param name="method">The HTTP method to use when executing the query.</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> QueryAsync(string query, HttpMethod method);

        /// <summary>
        /// Executes a query against the database in a single request. Multiple queries can be 
        /// passed in in the form of semicolon-delimited string.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="query">Queries to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <param name="method">The HTTP method to use when executing the query.</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> QueryAsync(string dbName, string query, HttpMethod method);

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