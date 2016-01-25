using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxData.Helpers;
using InfluxData.Net.InfluxDb.Helpers;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class ClientModuleBase
    {
        protected IInfluxDbRequestClient RequestClient { get; private set; }

        public ClientModuleBase(IInfluxDbRequestClient requestClient)
        {
            this.RequestClient = requestClient;
        }

        protected virtual async Task<IInfluxDataApiResponse> GetAndValidateQueryAsync(string query)
        {
            var response = await this.RequestClient.QueryAsync(query);
            response.ValidateQueryResponse();

            return response;
        }

        protected virtual async Task<IInfluxDataApiResponse> GetAndValidateQueryAsync(string dbName, string query)
        {
            var response = await this.RequestClient.QueryAsync(dbName, query);
            response.ValidateQueryResponse();

            return response;
        }

        protected virtual async Task<IEnumerable<Serie>> ResolveSingleGetSeriesResultAsync(string query)
        {
            var response = await this.RequestClient.QueryAsync(query);
            var series = ResolveSingleGetSeriesResult(response);

            return series;
        }

        protected virtual async Task<IEnumerable<Serie>> ResolveSingleGetSeriesResultAsync(string dbName, string query)
        {
            var response = await this.RequestClient.QueryAsync(dbName, query);
            var series = ResolveSingleGetSeriesResult(response);

            return series;
        }

        protected virtual IEnumerable<Serie> ResolveSingleGetSeriesResult(IInfluxDataApiResponse response)
        {
            var queryResponse = response.ReadAs<QueryResponse>().Validate();
            var result = queryResponse.Results.Single();
            Validate.IsNotNull(result, "result");

            var series = result.Series != null ? result.Series.ToList() : new List<Serie>();

            return series;
        }
    }
}
