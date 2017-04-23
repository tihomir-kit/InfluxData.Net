using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.Common.RequestClients;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Formatters;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using System;
using System.Linq;
using InfluxData.Net.Common.Enums;
using System.Collections.Generic;

namespace InfluxData.Net.InfluxDb.RequestClients
{
    public class InfluxDbRequestClient : RequestClientBase, IInfluxDbRequestClient
    {
        public InfluxDbRequestClient(IInfluxDbClientConfiguration configuration)
            : base(configuration, "InfluxData.Net.InfluxDb")
        {
        }

        public virtual async Task<IInfluxDataApiResponse> GetQueryAsync(string query, string dbName = null, string epochFormat = null, long? chunkSize = null)
        {
            return await this.QueryAsync(query, HttpMethod.Get, dbName, epochFormat, chunkSize).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> PostQueryAsync(string query, string dbName = null)
        {
            return await this.QueryAsync(query, HttpMethod.Post, dbName).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> PostAsync(WriteRequest writeRequest)
        {
            var httpContent = new StringContent(writeRequest.GetLines(), Encoding.UTF8, "text/plain");
            var requestParams = RequestParamsBuilder.BuildRequestParams(
                writeRequest.DbName,
                QueryParams.Precision, writeRequest.Precision,
                QueryParams.RetentionPolicy, writeRequest.RetentionPolicy);

            var result = await base.RequestAsync(HttpMethod.Post, RequestPaths.Write, requestParams, httpContent).ConfigureAwait(false);

            return new InfluxDataApiWriteResponse(result.StatusCode, result.Body);
        }

        public virtual async Task<IInfluxDataApiResponse> QueryAsync(string query, HttpMethod method, string dbName = null, string epochFormat = null, long? chunkSize = null)
        {
            var requestParams = RequestParamsBuilder.BuildQueryRequestParams(query, dbName, epochFormat);
            SetChunkSize(chunkSize, requestParams);

            return await base.RequestAsync(method, RequestPaths.Query, requestParams).ConfigureAwait(false);
        }

        protected virtual void SetChunkSize(long? chunkSize, IDictionary<string, string> requestParams)
        {
            if (chunkSize != null)
            {
                requestParams.Add(QueryParams.Chunked, "true");
                requestParams.Add(QueryParams.ChunkSize, chunkSize.ToString());
            }
        }

        public virtual IPointFormatter GetPointFormatter()
        {
            return new PointFormatter();
        }
    }
}