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

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class BasicClientModule : ClientModuleBase, IBasicClientModule
    {
        public BasicClientModule(IInfluxDbRequestClient requestClient)
            : base(requestClient)
        {
        }

        public async Task<IInfluxDbApiResponse> WriteAsync(string dbName, Point point, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds)
        {
            var response = await WriteAsync(dbName, new[] { point }, retenionPolicy, precision);

            return response;
        }

        public async Task<IInfluxDbApiResponse> WriteAsync(string dbName, Point[] points, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds)
        {
            var request = new WriteRequest(this.RequestClient.GetFormatter())
            {
                DbName = dbName,
                Points = points,
                RetentionPolicy = retenionPolicy,
                Precision = precision.GetParamValue()
            };

            var response = await this.RequestClient.Write(request);

            return response;
        }

        public async Task<IList<Serie>> QueryAsync(string dbName, string query)
        {
            var response = await this.RequestClient.Query(dbName, query);
            var queryResult = response.ReadAs<QueryResponse>();

            Validate.NotNull(queryResult, "queryResult");
            Validate.NotNull(queryResult.Results, "queryResult.Results");

            // Apparently a 200 OK can return an error in the results
            // https://github.com/influxdb/influxdb/pull/1813
            var error = queryResult.Results.Single().Error;
            if (error != null)
            {
                throw new InfluxDbApiException(HttpStatusCode.BadRequest, error);
            }

            var result = queryResult.Results.Single().Series;
            var series = result != null ? result.ToList() : new List<Serie>();

            return series;
        }

        public async Task<IList<IList<Serie>>> QueryAsync(string dbName, string[] queries)
        {
            var response = await this.RequestClient.Query(dbName, queries.ToSemicolonSpaceSeparatedString());
            var queryResult = response.ReadAs<QueryResponse>();
            var seriesList = new List<IList<Serie>>();

            Validate.NotNull(queryResult, "queryResult");
            Validate.NotNull(queryResult.Results, "queryResult.Results");

            // Apparently a 200 OK can return an error in the results
            // https://github.com/influxdb/influxdb/pull/1813
            foreach (var result in queryResult.Results)
            {
                if (result.Error != null)
                {
                    throw new InfluxDbApiException(HttpStatusCode.BadRequest, result.Error);
                }

                seriesList.Add(result.Series.ToList());
            }

            return seriesList;
        }
    }
}
