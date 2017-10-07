using InfluxData.Net.Common.Constants;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Helpers;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.ResponseParsers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class BasicClientModule : ClientModuleBase, IBasicClientModule
    {
        private readonly IBasicResponseParser _basicResponseParser;

        public virtual async Task<IEnumerable<Serie>> QueryAsync(string query, string dbName = null, string epochFormat = null, long? chunkSize = null)
        {
            var series = await base.ResolveSingleGetSeriesResultAsync(query, dbName, epochFormat, chunkSize).ConfigureAwait(false);

            return series;
        }

        public virtual async Task<IEnumerable<Serie>> QueryAsync(IEnumerable<string> queries, string dbName = null, string epochFormat = null, long? chunkSize = null)
        {
            var results = await base.ResolveGetSeriesResultAsync(queries.ToSemicolonSpaceSeparatedString(), dbName, epochFormat, chunkSize).ConfigureAwait(false);
            var series = _basicResponseParser.FlattenResultsSeries(results);

            return series;
        }

        public virtual async Task<IEnumerable<Serie>> QueryAsync(string queryTemplate, object parameters, string dbName = null, string epochFormat = null, long? chunkSize = null)
        {
            var query = queryTemplate.BuildQuery(parameters);

            return await this.QueryAsync(query, dbName, epochFormat, chunkSize);
        }

        public virtual async Task<IEnumerable<IEnumerable<Serie>>> MultiQueryAsync(IEnumerable<string> queries, string dbName = null, string epochFormat = null, long? chunkSize = null)
        {
            var results = await base.ResolveGetSeriesResultAsync(queries.ToSemicolonSpaceSeparatedString(), dbName, epochFormat, chunkSize).ConfigureAwait(false);
            var resultSeries = _basicResponseParser.MapResultsSeries(results);

            return resultSeries;
        }

        public BasicClientModule(IInfluxDbRequestClient requestClient, IBasicResponseParser basicResponseParser)
            : base(requestClient)
        {
            _basicResponseParser = basicResponseParser;
        }

        public virtual async Task<IInfluxDataApiResponse> WriteAsync(Point point, string dbName = null, string retentionPolicy = null, string precision = TimeUnit.Milliseconds)
        {
            var response = await WriteAsync(new [] { point }, dbName, retentionPolicy, precision).ConfigureAwait(false);

            return response;
        }

        public virtual async Task<IInfluxDataApiResponse> WriteAsync(IEnumerable<Point> points, string dbName = null, string retentionPolicy = null, string precision = TimeUnit.Milliseconds)
        {
            var request = new WriteRequest(base.RequestClient.GetPointFormatter())
            {
                DbName = dbName,
                Points = points,
                RetentionPolicy = retentionPolicy,
                Precision = precision
            };

            var response = await base.RequestClient.PostAsync(request).ConfigureAwait(false);

            return response;
        }
    }
}
