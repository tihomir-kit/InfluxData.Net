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
using InfluxData.Net.InfluxDb.RequestClients.Modules;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class BasicClientModule : ClientModuleBase, IBasicClientModule
    {
        private readonly IBasicRequestModule _basicRequestModule;

        public BasicClientModule(IInfluxDbRequestClient requestClient, IBasicRequestModule basicRequestModule)
            : base(requestClient)
        {
            _basicRequestModule = basicRequestModule;
        }

        public async Task<IInfluxDbApiResponse> WriteAsync(string dbName, Point point, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds)
        {
            return await WriteAsync(dbName, new[] { point }, retenionPolicy, precision);
        }

        public async Task<IInfluxDbApiResponse> WriteAsync(string dbName, Point[] points, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds)
        {
            var request = new WriteRequest(_requestClient.GetFormatter())
            {
                DbName = dbName,
                Points = points,
                RetentionPolicy = retenionPolicy,
                Precision = precision.GetParamValue()
            };

            return await _basicRequestModule.Write(request);
        }

        public async Task<IList<Serie>> QueryAsync(string dbName, string query)
        {
            var response = await _basicRequestModule.Query(dbName, query);
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

            return result != null ? result.ToList() : new List<Serie>();
        }

        public async Task<Pong> PingAsync()
        {
            var watch = Stopwatch.StartNew();
            var response = await _requestClient.PingAsync();

            watch.Stop();

            return new Pong
            {
                Version = response.Body,
                ResponseTime = watch.Elapsed,
                Success = true
            };
        }
    }
}
