using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfluxData.Net.Models;
using System.Diagnostics;
using InfluxData.Net.Constants;
using InfluxData.Net.Contracts;
using InfluxData.Net.Helpers;
using InfluxData.Net.Infrastructure.Configuration;
using InfluxData.Net.Infrastructure.Influx;
using InfluxData.Net.Enums;
using InfluxData.Net.Infrastructure.Formatters;

namespace InfluxData.Net.Client
{
    // TODO: extract base class from this

    internal class InfluxDbClientV09x : IInfluxDbClient
    {
        private const string UserAgent = "InfluxData.Net";

        private readonly InfluxDbClientConfiguration _configuration;

        public InfluxDbClientV09x(InfluxDbClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Database

        public async Task<InfluxDbApiResponse> CreateDatabase(string dbName)
        {
            var query = String.Format(QueryStatements.CreateDatabase, dbName);
            return await GetQueryAsync(requestParams: BuildQueryRequestParams(query));
        }

        public async Task<InfluxDbApiResponse> DropDatabase(string dbName)
        {
            var query = String.Format(QueryStatements.DropDatabase, dbName);
            return await GetQueryAsync(requestParams: BuildQueryRequestParams(query));
        }

        public async Task<InfluxDbApiResponse> ShowDatabases()
        {
            return await GetQueryAsync(requestParams: BuildQueryRequestParams(QueryStatements.ShowDatabases));
        }

        public async Task<InfluxDbApiResponse> DropSeries(string dbName, string serieName)
        {
            var query = String.Format(QueryStatements.DropSeries, serieName);
            return await GetQueryAsync(requestParams: BuildQueryRequestParams(query));
        }

        #endregion Database

        #region Basic Querying

        public async Task<InfluxDbApiWriteResponse> Write(WriteRequest writeRequest, string timePrecision)
        {
            var requestContent = new StringContent(writeRequest.GetLines(), Encoding.UTF8, "text/plain");
            var requestParams = BuildRequestParams(writeRequest.Database, QueryParams.Precision, timePrecision);
            var result = await RequestAsync(HttpMethod.Post, RequestPaths.Write, requestContent, requestParams);

            return new InfluxDbApiWriteResponse(result.StatusCode, result.Body);
        }

        public async Task<InfluxDbApiResponse> Query(string dbName, string query)
        {
            return await GetQueryAsync(requestParams: BuildQueryRequestParams(dbName, query));
        }

        #endregion Basic Querying

        #region Continuous Queries

        public async Task<InfluxDbApiResponse> CreateContinuousQuery(CqRequest cqRequest)
        {
            // TODO: perhaps extract subquery and query building to formatter
            var subQuery = String.Format(QueryStatements.CreateContinuousQuerySubQuery, String.Join(",", cqRequest.Downsamplers), 
                cqRequest.DsSerieName, cqRequest.SourceSerieName, cqRequest.Interval);

            if (cqRequest.Tags != null)
            {
                var tagsString = String.Join(",", cqRequest.Tags);
                if (!String.IsNullOrEmpty(tagsString))
                    String.Join(", ", subQuery, tagsString);
            }

            if (cqRequest.FillType != FillType.Null)
                String.Join(" ", subQuery, cqRequest.FillType.ToString().ToLower());

            var query = String.Format(QueryStatements.CreateContinuousQuery, cqRequest.CqName, cqRequest.DbName, subQuery);

            return await GetQueryAsync(requestParams: BuildQueryRequestParams(cqRequest.DbName, query));
        }

        public async Task<InfluxDbApiResponse> GetContinuousQueries(string dbName)
        {
            return await GetQueryAsync(requestParams: BuildQueryRequestParams(dbName, QueryStatements.ShowContinuousQueries));
        }

        public async Task<InfluxDbApiResponse> DeleteContinuousQuery(string dbName, string cqName)
        {
            var query = String.Format(QueryStatements.DropContinuousQuery, cqName, dbName);
            return await GetQueryAsync(requestParams: BuildQueryRequestParams(dbName, query));
        }

        #endregion Continuous Queries

        #region Other

        /// <summary>Pings the server.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> Ping()
        {
            return await RequestAsync(HttpMethod.Get, RequestPaths.Ping, includeAuthToQuery: false, headerIsBody: true);
        }

        public async Task<InfluxDbApiResponse> AlterRetentionPolicy(string policyName, string dbName, string duration, int replication)
        {
            var requestParams = BuildQueryRequestParams(String.Format(QueryStatements.AlterRetentionPolicy, policyName, dbName, duration, replication));

            return await GetQueryAsync(requestParams: requestParams);
        }

        #endregion Other

        #region Base

        public virtual IFormatter GetFormatter()
        {
            return new FormatterBase();
        }

        public virtual InfluxVersion GetVersion()
        {
            return InfluxVersion.Latest;
        }

        private HttpClient GetHttpClient()
        {
            return _configuration.BuildHttpClient();
        }

        private Dictionary<string, string> BuildQueryRequestParams(string query)
        {
            return new Dictionary<string, string>
            {
                { QueryParams.Query, query }
            };
        }

        private Dictionary<string, string> BuildQueryRequestParams(string dbName, string query)
        {
            return BuildRequestParams(dbName, QueryParams.Query, query);
        }

        private Dictionary<string, string> BuildRequestParams(string dbName, string requestType, string query)
        {
            return new Dictionary<string, string>
            {
                { QueryParams.Db, dbName },
                { requestType, query }
            };
        }

        private async Task<InfluxDbApiResponse> GetQueryAsync(Dictionary<string, string> requestParams)
        {
            return await GetQueryAsync(requestParams: requestParams);
        }

        private async Task<InfluxDbApiResponse> GetQueryAsync(
            HttpContent content = null,
            Dictionary<string, string> requestParams = null,
            bool includeAuthToQuery = true,
            bool headerIsBody = false)
        {
            return await RequestAsync(HttpMethod.Get, RequestPaths.Query, content, requestParams, includeAuthToQuery, headerIsBody);
        }

        private async Task<InfluxDbApiResponse> RequestAsync(
            HttpMethod method, 
            string path,
            HttpContent content = null,
            Dictionary<string, string> requestParams = null,
            bool includeAuthToQuery = true,
            bool headerIsBody = false)
        {
            var response = await RequestInnerAsync(null,
                HttpCompletionOption.ResponseHeadersRead,
                CancellationToken.None,
                method,
                path,
                content,
                requestParams,
                includeAuthToQuery);

            string responseContent = String.Empty;

            if (!headerIsBody)
            {
                responseContent = await response.Content.ReadAsStringAsync();
            }
            else
            {
                IEnumerable<string> values;

                if (response.Headers.TryGetValues("X-Influxdb-Version", out values))
                {
                    responseContent = values.First();
                }
            }

            HandleIfErrorResponse(response.StatusCode, responseContent);

#if DEBUG
            Debug.WriteLine("[Response] {0}", response.ToJson());
            Debug.WriteLine("[ResponseData] {0}", responseContent);
#endif

            return new InfluxDbApiResponse(response.StatusCode, responseContent);
        }

        private async Task<HttpResponseMessage> RequestInnerAsync(
            TimeSpan? requestTimeout,
            HttpCompletionOption completionOption, 
            CancellationToken cancellationToken, 
            HttpMethod method, 
            string path,
            HttpContent content = null, 
            Dictionary<string, string> extraParams = null, 
            bool includeAuthToQuery = true)
        {
            HttpClient client = GetHttpClient();

            if (requestTimeout.HasValue)
            {
                client.Timeout = requestTimeout.Value;
            }

            StringBuilder uri = BuildUri(path, extraParams, includeAuthToQuery);
            HttpRequestMessage request = BuildRequest(method, content, uri);

#if DEBUG
            Debug.WriteLine("[Request] {0}", request.ToJson());
            if (content != null)
            {
                Debug.WriteLine("[RequestData] {0}", content.ReadAsStringAsync().Result);
            }
#endif

            return await client.SendAsync(request, completionOption, cancellationToken);
        }

        private StringBuilder BuildUri(string path, Dictionary<string, string> requestParams, bool includeAuthToQuery)
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.AppendFormat("{0}{1}", _configuration.EndpointBaseUri, path);

            if (includeAuthToQuery)
            {
                urlBuilder.AppendFormat("?{0}={1}&{2}={3}", QueryParams.Username, HttpUtility.UrlEncode(_configuration.Username), QueryParams.Password, HttpUtility.UrlEncode(_configuration.Password));
            }

            if (requestParams != null && requestParams.Count > 0)
            {
                var keyValues = new List<string>(requestParams.Count);
                keyValues.AddRange(requestParams.Select(param => String.Format("{0}={1}", param.Key, param.Value)));
                urlBuilder.AppendFormat("{0}{1}", includeAuthToQuery ? "&" : "?", String.Join("&", keyValues));
            }

            return urlBuilder;
        }

        private static HttpRequestMessage BuildRequest(HttpMethod method, HttpContent content, StringBuilder urlBuilder)
        {
            var request = new HttpRequestMessage(method, urlBuilder.ToString());
            request.Headers.Add("User-Agent", UserAgent);
            request.Headers.Add("Accept", "application/json");

            request.Content = content;

            return request;
        }

        private void HandleIfErrorResponse(HttpStatusCode statusCode, string responseBody)
        {
            if (statusCode < HttpStatusCode.OK || statusCode >= HttpStatusCode.BadRequest)
            {
#if DEBUG
                Debug.WriteLine(String.Format("[Error] {0} {1}", statusCode, responseBody));
#endif
                throw new InfluxDbApiException(statusCode, responseBody);
            }
        }

        #endregion Base
    }

    internal delegate void ApiResponseErrorHandlingDelegate(HttpStatusCode statusCode, string responseBody);
}