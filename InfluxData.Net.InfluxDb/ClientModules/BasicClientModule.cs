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

        public virtual async Task<IInfluxDataApiResponse> WriteAsync(string dbName, Point point, string retenionPolicy = null, TimeUnit precision = TimeUnit.Milliseconds)
        {
            var response = await WriteAsync(dbName, new [] { point }, retenionPolicy, precision).ConfigureAwait(false);

            return response;
        }

        public virtual async Task<IInfluxDataApiResponse> WriteAsync(string dbName, IEnumerable<Point> points, string retenionPolicy = null, TimeUnit precision = TimeUnit.Milliseconds)
        {
            var request = new WriteRequest(base.RequestClient.GetPointFormatter())
            {
                DbName = dbName,
                Points = points,
                RetentionPolicy = retenionPolicy,
                Precision = precision.GetParamValue()
            };

            var response = await base.RequestClient.PostAsync(request).ConfigureAwait(false);

            return response;
        }

        public virtual async Task<IEnumerable<Serie>> QueryAsync(string dbName, string query)
        {
            var response = await base.RequestClient.QueryAsync(dbName, query).ConfigureAwait(false);
            var series = await base.ResolveSingleGetSeriesResultAsync(dbName, query).ConfigureAwait(false);
            return series;
        }

        public virtual async Task<IEnumerable<Serie>> QueryAsync(string dbName, IEnumerable<string> queries)
        {
            var response = await base.RequestClient.QueryAsync(dbName, queries.ToSemicolonSpaceSeparatedString()).ConfigureAwait(false);
            var results = response.ReadAs<QueryResponse>().Validate().Results;
            var series = _basicResponseParser.FlattenResultsSeries(results);
            return series;
        }

        public virtual async Task<IEnumerable<IEnumerable<Serie>>> MultiQueryAsync(string dbName, IEnumerable<string> queries)
        {
            var response = await base.RequestClient.QueryAsync(dbName, queries.ToSemicolonSpaceSeparatedString()).ConfigureAwait(false);
            var results = response.ReadAs<QueryResponse>().Validate().Results;
            var resultSeries = _basicResponseParser.MapResultsSeries(results);

            return resultSeries;
        }
    }
}
