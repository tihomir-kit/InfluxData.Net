using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.Common.Constants;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public interface IBasicClientModule
    {
        /// <summary>
        /// Executes a query against the database. If chunkSize is specified, responses 
        /// will be broken down by number of returned rows. 
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <param name="dbName">Database name. (OPTIONAL)</param>
        /// <param name="epochFormat">Epoch timestamp format. (OPTIONAL)</param>
        /// <param name="chunkSize">Maximum number of rows per chunk. (OPTIONAL)</param>
        /// <returns></returns>
        Task<IEnumerable<Serie>> QueryAsync(string query, string dbName = null, string epochFormat = null, long? chunkSize = null);

        /// <summary>
        /// Executes multiple queries against the database in a single request and extracts and flattens
        /// the series from all results into a single <see cref="Serie"/> collection. If chunkSize is specified, responses 
        /// will be broken down by number of returned rows. 
        /// </summary>
        /// <param name="queries">Queries to execute.</param>
        /// <param name="dbName">Database name. (OPTIONAL)</param>
        /// <param name="epochFormat">Epoch timestamp format. (OPTIONAL)</param>
        /// <param name="chunkSize">Maximum number of rows per chunk. (OPTIONAL)</param>
        /// <returns></returns>
        Task<IEnumerable<Serie>> QueryAsync(IEnumerable<string> queries, string dbName = null, string epochFormat = null, long? chunkSize = null);

        /// <summary>
        /// Executes a parameterized query against the database. If chunkSize is specified, responses 
        /// will be broken down by number of returned rows. 
        /// </summary>
        /// <param name="queryTemplate">Query template to use to build a full query using params.</param>
        /// <param name="parameters">The parameters to pass, if any.</param>
        /// <param name="dbName">Database name. (OPTIONAL)</param>
        /// <param name="epochFormat">Epoch timestamp format. (OPTIONAL)</param>
        /// <param name="chunkSize">Maximum number of rows per chunk. (OPTIONAL)</param>
        /// <returns></returns>
        Task<IEnumerable<Serie>> QueryAsync(string queryTemplate, object parameters, string dbName = null, string epochFormat = null, long? chunkSize = null);

        /// <summary>
        /// Executes multiple queries against the database in a single request. If chunkSize is specified, responses 
        /// will be broken down by number of returned rows. 
        /// </summary>
        /// <param name="queries">Queries to execute.</param>
        /// <param name="dbName">Database name. (OPTIONAL)</param>
        /// <param name="epochFormat">Epoch timestamp format. (OPTIONAL)</param>
        /// <param name="chunkSize">Maximum number of rows per chunk. (OPTIONAL)</param>
        /// <returns></returns>
        Task<IEnumerable<IEnumerable<Serie>>> MultiQueryAsync(IEnumerable<string> queries, string dbName = null, string epochFormat = null, long? chunkSize = null);

        /// <summary>
        /// Writes a single serie point to the database.
        /// </summary>
        /// <param name="point">A serie <see cref="{Point}" />.</param>
        /// <param name="dbName">Database name.</param>
        /// <param name="retentionPolicy">The retention policy.</param>
        /// <param name="precision">InfluxDb time precision to use (defaults to 'ms')</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> WriteAsync(Point point, string dbName = null, string retentionPolicy = null, string precision = TimeUnit.Milliseconds);

        /// <summary>
        /// Writes multiple serie points to the database.
        /// </summary>
        /// <param name="points">A serie <see cref="Array" />.</param>
        /// <param name="dbName">Database name.</param>
        /// <param name="retentionPolicy">The retention policy.</param>
        /// <param name="precision">InfluxDb time precision to use (defaults to 'ms')</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> WriteAsync(IEnumerable<Point> points, string dbName = null, string retentionPolicy = null, string precision = TimeUnit.Milliseconds);
    }
}