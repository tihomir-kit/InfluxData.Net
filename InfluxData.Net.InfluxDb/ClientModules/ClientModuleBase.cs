using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.RequestClients;
using System.Threading.Tasks;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class ClientModuleBase
    {
        protected IInfluxDbRequestClient RequestClient { get; private set; }

        public ClientModuleBase(IInfluxDbRequestClient requestClient)
        {
            this.RequestClient = requestClient;
        }

        protected async Task<IInfluxDbApiResponse> GetQueryAsync(string query)
        {
            var requestParams = RequestParamsBuilder.BuildQueryRequestParams(query);
            var response = await this.RequestClient.GetQueryAsync(requestParams: requestParams);

            return response;
        }

        protected async Task<IInfluxDbApiResponse> GetQueryAsync(string dbName, string query)
        {
            var requestParams = RequestParamsBuilder.BuildQueryRequestParams(dbName, query);
            var response = await this.RequestClient.GetQueryAsync(requestParams: requestParams);

            return response;
        }
    }
}
