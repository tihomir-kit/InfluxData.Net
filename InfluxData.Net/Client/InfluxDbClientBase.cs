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
    internal class InfluxDbClientBase : IInfluxDbClient
    {
        private const string UserAgent = "InfluxData.Net";

        private readonly InfluxDbClientConfiguration _configuration;

        public InfluxDbClientBase(InfluxDbClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Database

        public async Task<InfluxDbApiResponse> CreateDatabase(string dbName)
        {
            var query = String.Format(QueryStatements.CreateDatabase, dbName);
            return await RequestAsync(HttpMethod.Get, "query", null, BuildQueryParams(query));
        }

        public async Task<InfluxDbApiResponse> DropDatabase(string name)
        {
            var query = String.Format(QueryStatements.DropDatabase, name);
            return await RequestAsync(HttpMethod.Get, "query", null, BuildQueryParams(query));
        }

        public async Task<InfluxDbApiResponse> ShowDatabases()
        {
            return await RequestAsync(HttpMethod.Get, "query", null, BuildQueryParams(QueryStatements.ShowDatabases));
        }

        #endregion Database

        #region Basic Querying

        public async Task<InfluxDbApiWriteResponse> Write(WriteRequest request, string timePrecision)
        {
            var content = new StringContent(request.GetLines(), Encoding.UTF8, "text/plain");
            var result = await RequestAsync(HttpMethod.Post, "write", content,
                new Dictionary<string, string>
                {
                    { QueryParams.Db, request.Database },
                    { QueryParams.Precision, timePrecision }
                });

            return new InfluxDbApiWriteResponse(result.StatusCode, result.Body);
        }

        public async Task<InfluxDbApiResponse> Query(string dbName, string query)
        {
            return await RequestAsync(HttpMethod.Get, "query", null, BuildQueryParams(dbName, query));
        }

        #endregion Basic Querying

        #region Continuous Queries

        public async Task<InfluxDbApiResponse> CreateContinuousQuery(CqRequest cqRequest)
        {
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

            return await RequestAsync(HttpMethod.Get, "query", null, BuildQueryParams(cqRequest.DbName, query));
        }

        public async Task<InfluxDbApiResponse> GetContinuousQueries(string dbName)
        {
            return await RequestAsync(HttpMethod.Get, "query", null, BuildQueryParams(dbName, QueryStatements.ShowContinuousQueries));
        }

        public async Task<InfluxDbApiResponse> DeleteContinuousQuery(string dbName, string cqName)
        {
            var query = String.Format(QueryStatements.DropContinuousQuery, cqName, dbName);
            return await RequestAsync(HttpMethod.Get, "query", null, BuildQueryParams(dbName, query));
        }

        #endregion Continuous Queries

        #region Series

        public async Task<InfluxDbApiResponse> DropSeries(string database, string name)
        {
            return await RequestAsync(HttpMethod.Get, "query", null,
                new Dictionary<string, string>
                {
                    { QueryParams.Db, database },
                    { QueryParams.Query, String.Format(QueryStatements.DropSeries, name) }
                });
        }

        #endregion Series

        #region Other

        /// <summary>Pings the server.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> Ping()
        {
            return await RequestAsync(HttpMethod.Get, "ping", null, null, false, true);
        }

        public async Task<InfluxDbApiResponse> AlterRetentionPolicy(string policyName, string dbName, string duration, int replication)
        {
            return await RequestAsync(HttpMethod.Get, "query", null,
                new Dictionary<string, string>
                {
                    {QueryParams.Query, String.Format(QueryStatements.AlterRetentionPolicy, policyName, dbName, duration, replication) }
                });
        }

        #endregion Other

        #region Base

        public virtual IFormatter GetFormatter()
        {
            return new FormatterBase();
        }

        public virtual InfluxVersion GetVersion()
        {
            return InfluxVersion.v09x;
        }

        private HttpClient GetHttpClient()
        {
            return _configuration.BuildHttpClient();
        }

        private async Task<InfluxDbApiResponse> RequestAsync(
            HttpMethod method, 
            string path,
            HttpContent content = null,
            Dictionary<string, string> extraParams = null,
            bool includeAuthToQuery = true,
            bool headerIsBody = false)
        {
            var response = await RequestInnerAsync(null,
                HttpCompletionOption.ResponseHeadersRead,
                CancellationToken.None,
                method,
                path,
                content,
                extraParams,
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

            Debug.WriteLine("[Response] {0}", response.ToJson());
            Debug.WriteLine("[ResponseData] {0}", responseContent);

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
            HttpRequestMessage request = PrepareRequest(method, content, uri);

            Debug.WriteLine("[Request] {0}", request.ToJson());
            if (content != null)
            {
                Debug.WriteLine("[RequestData] {0}", content.ReadAsStringAsync().Result);
            }

            return await client.SendAsync(request, completionOption, cancellationToken);
        }

        private StringBuilder BuildUri(string path, Dictionary<string, string> extraParams, bool includeAuthToQuery)
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.AppendFormat("{0}{1}", _configuration.EndpointBaseUri, path);

            if (includeAuthToQuery)
            {
                urlBuilder.AppendFormat("?{0}={1}&{2}={3}", QueryParams.Username, HttpUtility.UrlEncode(_configuration.Username), QueryParams.Password, HttpUtility.UrlEncode(_configuration.Password));
            }

            if (extraParams != null && extraParams.Count > 0)
            {
                var keyValues = new List<string>(extraParams.Count);
                keyValues.AddRange(extraParams.Select(param => String.Format("{0}={1}", param.Key, param.Value)));
                urlBuilder.AppendFormat("{0}{1}", includeAuthToQuery ? "&" : "?", String.Join("&", keyValues));
            }

            return urlBuilder;
        }

        private static HttpRequestMessage PrepareRequest(HttpMethod method, HttpContent content, StringBuilder urlBuilder)
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
                Debug.WriteLine(String.Format("[Error] {0} {1}", statusCode, responseBody));
                throw new InfluxDbApiException(statusCode, responseBody);
            }
        }

        #endregion Base


        // TODO: extract somewhere else this helper method
        private Dictionary<string, string> BuildQueryParams(string query)
        {
            return new Dictionary<string, string>
            {
                {QueryParams.Query, query}
            };
        }

        // TODO: extract somewhere else this helper method
        private Dictionary<string, string> BuildQueryParams(string dbName, string query)
        {
            return new Dictionary<string, string>
            {
                {QueryParams.Db, dbName},
                {QueryParams.Query, query}
            };
        }
    }

    internal delegate void ApiResponseErrorHandlingDelegate(HttpStatusCode statusCode, string responseBody);
}