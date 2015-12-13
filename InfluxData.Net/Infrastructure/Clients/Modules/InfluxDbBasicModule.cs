using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfluxData.Net.Infrastructure.Configuration;
using InfluxData.Net.Infrastructure.Influx;
using InfluxData.Net.Models;
using System.Net.Http;

using System.Threading.Tasks;
using InfluxData.Net.Constants;

namespace InfluxData.Net.Infrastructure.Clients.Modules
{
    internal class InfluxDbBasicModule : InfluxDbModule, IInfluxDbBasicModule
    {
        public InfluxDbBasicModule(IInfluxDbClient client) 
            : base(client)
        {
        }

        public async Task<InfluxDbApiWriteResponse> Write(WriteRequest writeRequest, string timePrecision)
        {
            var requestContent = new StringContent(writeRequest.GetLines(), Encoding.UTF8, "text/plain");
            var requestParams = InfluxDbClientUtility.BuildRequestParams(writeRequest.Database, QueryParams.Precision, timePrecision);
            var result = await this.Client.PostWriteAsync(requestParams: requestParams, content: requestContent);

            return new InfluxDbApiWriteResponse(result.StatusCode, result.Body);
        }

        public async Task<InfluxDbApiResponse> Query(string dbName, string query)
        {
            return await this.Client.GetQueryAsync(requestParams: InfluxDbClientUtility.BuildQueryRequestParams(dbName, query));
        }
    }
}
