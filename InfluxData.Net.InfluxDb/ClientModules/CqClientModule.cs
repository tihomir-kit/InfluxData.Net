using System.Linq;
using System.Net;
using System.Threading.Tasks;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.RequestClients.Modules;
using System;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class CqClientModule : ClientModuleBase, ICqClientModule
    {
        private readonly ICqRequestModule _cqRequestModule;

        public CqClientModule(IInfluxDbRequestClient requestClient, ICqRequestModule cqRequestModule)
            : base(requestClient)
        {
            _cqRequestModule = cqRequestModule;
        }

        public async Task<IInfluxDbApiResponse> CreateContinuousQueryAsync(ContinuousQuery continuousQuery)
        {
            return await CreateContinuousQueryAsync(continuousQuery);
        }

        public async Task<Serie> GetContinuousQueriesAsync(string dbName)
        {
            var response = await _cqRequestModule.GetContinuousQueries(dbName);
            var queryResult = response.ReadAs<QueryResponse>();//.Results.Single().Series;

            Validate.NotNull(queryResult, "queryResult");
            Validate.NotNull(queryResult.Results, "queryResult.Results");

            // Apparently a 200 OK can return an error in the results
            // https://github.com/influxdb/influxdb/pull/1813
            var error = queryResult.Results.Single().Error;
            if (error != null)
            {
                throw new InfluxDbApiException(HttpStatusCode.BadRequest, error);
            }

            var series = queryResult.Results.Single().Series;

            return series != null ? series.Where(p => p.Name == dbName).FirstOrDefault() : new Serie();
        }

        public async Task<IInfluxDbApiResponse> DeleteContinuousQueryAsync(string dbName, string cqName)
        {
            return await _cqRequestModule.DeleteContinuousQuery(dbName, cqName);
        }

        public async Task<IInfluxDbApiResponse> BackfillAsync(string dbName, Backfill backfill)
        {
            return await _cqRequestModule.Backfill(dbName, backfill);
        }
    }
}
