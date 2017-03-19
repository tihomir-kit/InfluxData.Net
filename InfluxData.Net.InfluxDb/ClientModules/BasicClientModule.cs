using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxData.Helpers;
using InfluxData.Net.InfluxDb.Helpers;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.ResponseParsers;
using InfluxData.Net.Common.Constants;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class BasicClientModule : ClientModuleBase, IBasicClientModule
    {
        private readonly IBasicResponseParser _basicResponseParser;

        public BasicClientModule(IInfluxDbRequestClient requestClient, IBasicResponseParser basicResponseParser)
            : base(requestClient)
        {
            _basicResponseParser = basicResponseParser;
        }

        public virtual async Task<IInfluxDataApiResponse> WriteAsync(string dbName, Point point, string retentionPolicy = null, string precision = TimeUnit.Milliseconds)
        {
            var response = await WriteAsync(dbName, new [] { point }, retentionPolicy, precision).ConfigureAwait(false);

            return response;
        }

        public virtual async Task<IInfluxDataApiResponse> WriteAsync(string dbName, IEnumerable<Point> points, string retentionPolicy = null, string precision = TimeUnit.Milliseconds)
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

        public virtual async Task<IEnumerable<Serie>> QueryAsync(string dbName, string query)
        {
            var series = await base.ResolveSingleGetSeriesResultAsync(dbName, query).ConfigureAwait(false);

            return series;
        }

        public virtual async Task<IEnumerable<Serie>> QueryAsync(string dbName, IEnumerable<string> queries)
        {
            var results = await base.ResolveGetSeriesResultAsync(dbName, queries.ToSemicolonSpaceSeparatedString()).ConfigureAwait(false);
            var series = _basicResponseParser.FlattenResultsSeries(results);

            return series;
        }

        public virtual async Task<IEnumerable<IEnumerable<Serie>>> MultiQueryAsync(string dbName, IEnumerable<string> queries)
        {
            var results = await base.ResolveGetSeriesResultAsync(dbName, queries.ToSemicolonSpaceSeparatedString()).ConfigureAwait(false);
            var resultSeries = _basicResponseParser.MapResultsSeries(results);

            return resultSeries;
        }

        public virtual async Task<IEnumerable<Serie>> QueryChunkedAsync(string dbName, string query, long chunkSize = 10000)
        {
            var series = await base.ResolveSingleGetSeriesResultChunkedAsync(dbName, query, chunkSize).ConfigureAwait(false);
            return series;
        }

        public virtual async Task<IEnumerable<Serie>> QueryChunkedAsync(string dbName, IEnumerable<string> queries, long chunkSize = 10000)
        {
            var results = await base.ResolveGetSeriesResultChunkedAsync(dbName, queries.ToSemicolonSpaceSeparatedString(), chunkSize).ConfigureAwait(false);
            var series = _basicResponseParser.FlattenResultsSeries(results);
            return series;
        }
    }
}
