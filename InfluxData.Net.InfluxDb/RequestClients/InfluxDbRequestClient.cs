using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.Common.RequestClients;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Formatters;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Enums;

namespace InfluxData.Net.InfluxDb.RequestClients
{
    public class InfluxDbRequestClient : RequestClientBase, IInfluxDbRequestClient
    {
        private IInfluxDbClientConfiguration _influxDbConfiguration;

        public InfluxDbRequestClient(IInfluxDbClientConfiguration configuration)
            : base(configuration, "InfluxData.Net.InfluxDb")
        {
            _influxDbConfiguration = configuration;
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
            var requestParams = RequestParamsBuilder.BuildRequestParams(
                writeRequest.DbName,
                QueryParams.Precision, writeRequest.Precision,
                QueryParams.RetentionPolicy, writeRequest.RetentionPolicy
            );
            var httpContent = new StringContent(writeRequest.GetLines(), Encoding.UTF8, "text/plain");

            var result = await base.RequestAsync(HttpMethod.Post, RequestPaths.Write, requestParams, httpContent).ConfigureAwait(false);

            return new InfluxDataApiWriteResponse(result.StatusCode, result.Body);
        }

        public virtual async Task<IInfluxDataApiResponse> QueryAsync(string query, HttpMethod method, string dbName = null, string epochFormat = null, long? chunkSize = null)
        {
            if (_influxDbConfiguration.QueryLocation == QueryLocation.Uri)
            {
                return await this.QueryUriAsync(query, method, dbName, epochFormat, chunkSize).ConfigureAwait(false);
            }
            else
            {
                return await this.QueryFormDataAsync(query, HttpMethod.Post, dbName, epochFormat, chunkSize).ConfigureAwait(false);
            }
        }

        protected virtual async Task<IInfluxDataApiResponse> QueryUriAsync(string query, HttpMethod method, string dbName = null, string epochFormat = null, long? chunkSize = null)
        {
            var requestParams = RequestParamsBuilder.BuildQueryRequestParams(query, dbName, epochFormat, chunkSize);

            return await base.RequestAsync(method, RequestPaths.Query, requestParams).ConfigureAwait(false);
        }

        protected virtual async Task<IInfluxDataApiResponse> QueryFormDataAsync(string query, HttpMethod method, string dbName = null, string epochFormat = null, long? chunkSize = null)
        {
            var requestParams = RequestParamsBuilder.BuildRequestParams(dbName, epochFormat, chunkSize);
            var httpContent = query.ToMultipartHttpContent(QueryParams.Query);

            return await base.RequestAsync(method, RequestPaths.Query, requestParams, httpContent).ConfigureAwait(false);
        }

        public virtual IPointFormatter GetPointFormatter()
        {
            return new PointFormatter();
        }
    }
}