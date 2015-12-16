using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Constants;

namespace InfluxData.Net.InfluxDb.RequestClients.Modules
{
    internal class BasicRequestModule : RequestModuleBase, IBasicRequestModule
    {
        public BasicRequestModule(IInfluxDbRequestClient requestClient) 
            : base(requestClient)
        {
        }

        public async Task<InfluxDbApiWriteResponse> Write(WriteRequest writeRequest, string timePrecision)
        {
            var requestContent = new StringContent(writeRequest.GetLines(), Encoding.UTF8, "text/plain");
            var requestParams = RequestClientUtility.BuildRequestParams(writeRequest.Database, QueryParams.Precision, timePrecision);
            var result = await this.RequestClient.PostDataAsync(requestParams: requestParams, content: requestContent);

            return new InfluxDbApiWriteResponse(result.StatusCode, result.Body);
        }

        public async Task<InfluxDbApiResponse> Query(string dbName, string query)
        {
            return await this.RequestClient.GetQueryAsync(requestParams: RequestClientUtility.BuildQueryRequestParams(dbName, query));
        }
    }
}
