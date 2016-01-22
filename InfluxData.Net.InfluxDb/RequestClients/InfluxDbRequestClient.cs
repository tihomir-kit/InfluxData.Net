using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Formatters;
using InfluxData.Net.InfluxDb.Infrastructure;
using System.Diagnostics;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.Common.RequestClients;
using InfluxData.Net.InfluxDb.Models;

namespace InfluxData.Net.InfluxDb.RequestClients
{
    public class InfluxDbRequestClient : RequestClientBase, IInfluxDbRequestClient
    {
        public InfluxDbRequestClient(IInfluxDbClientConfiguration configuration)
            : base(configuration.EndpointUri.AbsoluteUri, configuration.Username, configuration.Password, "InfluxData.Net.InfluxDb")
        {
        }

        public virtual async Task<IInfluxDataApiResponse> QueryAsync(string query)
        {
            var requestParams = RequestParamsBuilder.BuildQueryRequestParams(query);
            return await base.RequestAsync(HttpMethod.Get, RequestPaths.Query, requestParams);

        }

        public virtual async Task<IInfluxDataApiResponse> QueryAsync(string dbName, string query)
        {
            var requestParams = RequestParamsBuilder.BuildQueryRequestParams(dbName, query);
            return await base.RequestAsync(HttpMethod.Get, RequestPaths.Query, requestParams);
        }

        public virtual async Task<IInfluxDataApiResponse> PostAsync(WriteRequest writeRequest)
        {
            var requestContent = new StringContent(writeRequest.GetLines(), Encoding.UTF8, "text/plain");
            var requestParams = RequestParamsBuilder.BuildRequestParams(writeRequest.DbName, QueryParams.Precision, writeRequest.Precision);
            var result = await base.RequestAsync(HttpMethod.Post, RequestPaths.Write, requestParams, requestContent);

            return new InfluxDataApiWriteResponse(result.StatusCode, result.Body);
        }

        public virtual IPointFormatter GetPointFormatter()
        {
            return new PointFormatter();
        }

    }
}