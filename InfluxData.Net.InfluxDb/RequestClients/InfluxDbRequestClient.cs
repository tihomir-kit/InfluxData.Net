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

namespace InfluxData.Net.InfluxDb.RequestClients
{
    public class InfluxDbRequestClient : RequestClientBase, IInfluxDbRequestClient
    {
        public InfluxDbRequestClient(IInfluxDbClientConfiguration configuration)
            : base(configuration, "InfluxData.Net.InfluxDb")
        {
        }

        public virtual async Task<IInfluxDataApiResponse> GetQueryAsync(string query)
        {
            return await this.QueryAsync(query, HttpMethod.Get).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> PostQueryAsync(string query)
        {
            return await this.QueryAsync(query, HttpMethod.Post).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> QueryAsync(string query, HttpMethod method)
        {
            var requestParams = RequestParamsBuilder.BuildQueryRequestParams(query);
            return await base.RequestAsync(method, RequestPaths.Query, requestParams).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> GetQueryAsync(string dbName, string query, string epochFormat)
        {
            return await this.QueryAsync(dbName, query, epochFormat, HttpMethod.Get).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> PostQueryAsync(string dbName, string query)
        {
            return await this.QueryAsync(dbName, query, null, HttpMethod.Post).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> QueryAsync(string dbName, string query, string epochFormat, HttpMethod method)
        {
            var requestParams = RequestParamsBuilder.BuildRequestParams(dbName, QueryParams.Query, query, QueryParams.Epoch, epochFormat);
            return await base.RequestAsync(method, RequestPaths.Query, requestParams).ConfigureAwait(false);
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

        public virtual IPointFormatter GetPointFormatter()
        {
            return new PointFormatter();
        }
    }
}