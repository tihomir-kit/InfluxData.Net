using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.ResponseParsers;
using InfluxData.Net.InfluxDb.Helpers;

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

        public virtual async Task<IInfluxDbApiResponse> WriteAsync(string dbName, Point point, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds)
        {
            var response = await WriteAsync(dbName, new [] { point }, retenionPolicy, precision);

            return response;
        }

        public virtual async Task<IInfluxDbApiResponse> WriteAsync(string dbName, IEnumerable<Point> points, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds)
        {
            var request = new WriteRequest(this.RequestClient.GetPointFormatter())
            {
                DbName = dbName,
                Points = points,
                RetentionPolicy = retenionPolicy,
                Precision = precision.GetParamValue()
            };

            var response = await this.RequestClient.WriteAsync(request);

            return response;
        }

        public virtual async Task<IEnumerable<Serie>> QueryAsync(string dbName, string query)
        {
            var response = await base.RequestClient.QueryAsync(dbName, query);
            var series = await base.ResolveSingleGetSeriesResultAsync(dbName, query);

            return series;
        }

        public virtual async Task<IEnumerable<Serie>> QueryAsync(string dbName, IEnumerable<string> queries)
        {
            var response = await base.RequestClient.QueryAsync(dbName, queries.ToSemicolonSpaceSeparatedString());
            var results = response.ReadAs<QueryResponse>().Validate().Results;
            var series = _basicResponseParser.FlattenResultsSeries(results);

            return series;
        }

        public virtual async Task<IEnumerable<IEnumerable<Serie>>> MultiQueryAsync(string dbName, IEnumerable<string> queries)
        {
            var response = await base.RequestClient.QueryAsync(dbName, queries.ToSemicolonSpaceSeparatedString());
            var results = response.ReadAs<QueryResponse>().Validate().Results;
            var resultSeries = _basicResponseParser.MapResultsSeries(results);

            return resultSeries;
        }


    }
}
