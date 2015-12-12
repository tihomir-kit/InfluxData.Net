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

        private readonly ApiResponseErrorHandlingDelegate _defaultErrorHandlingDelegate = (statusCode, body) =>
        {
            if (statusCode < HttpStatusCode.OK || statusCode >= HttpStatusCode.BadRequest)
            {
                Debug.WriteLine(String.Format("[Error] {0} {1}", statusCode, body));
                throw new InfluxDbApiException(statusCode, body);
            }
        };

        public InfluxDbClientBase(InfluxDbClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Database

        /// <summary>Creates the database.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> CreateDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string dbName)
        {
            var query = String.Format(QueryStatements.CreateDatabase, dbName);
            return await RequestAsync(errorHandlers, HttpMethod.Get, "query", null, BuildQueryParams(query));
        }

        /// <summary>Drops the database.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> DropDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string name)
        {
            var query = String.Format(QueryStatements.DropDatabase, name);
            return await RequestAsync(errorHandlers, HttpMethod.Get, "query", null, BuildQueryParams(query));
        }

        /// <summary>Queries the list of databases.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> ShowDatabases(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "query", null, BuildQueryParams(QueryStatements.ShowDatabases));
        }

        #endregion Database

        #region Basic Querying

        /// <summary>Writes the request to the endpoint.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="request">The request.</param>
        /// <param name="timePrecision">The time precision.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiWriteResponse> Write(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, WriteRequest request, string timePrecision)
        {
            var content = new StringContent(request.GetLines(), Encoding.UTF8, "text/plain");
            var result = await RequestAsync(errorHandlers, HttpMethod.Post, "write", content,
                new Dictionary<string, string>
                {
                    { QueryParams.Db, request.Database },
                    { QueryParams.Precision, timePrecision }
                });

            return new InfluxDbApiWriteResponse(result.StatusCode, result.Body);
        }

        /// <summary>Queries the endpoint.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="dbName">The name.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> Query(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string dbName, string query)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "query", null, BuildQueryParams(dbName, query));
        }

        #endregion Basic Querying

        #region Continuous Queries

        public async Task<InfluxDbApiResponse> CreateContinuousQuery(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, CqRequest cqRequest)
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

            return await RequestAsync(errorHandlers, HttpMethod.Get, "query", null, BuildQueryParams(cqRequest.DbName, query));
        }

        public async Task<InfluxDbApiResponse> GetContinuousQueries(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string dbName)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "query", null, BuildQueryParams(dbName, QueryStatements.ShowContinuousQueries));
        }

        public async Task<InfluxDbApiResponse> DeleteContinuousQuery(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string dbName, string cqName)
        {
            var query = String.Format(QueryStatements.DropContinuousQuery, cqName, dbName);
            return await RequestAsync(errorHandlers, HttpMethod.Get, "query", null, BuildQueryParams(dbName, query));
        }

        #endregion Continuous Queries

        #region Series

        public async Task<InfluxDbApiResponse> DropSeries(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string name)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "query", null,
                new Dictionary<string, string>
                {
                    { QueryParams.Db, database },
                    { QueryParams.Query, String.Format(QueryStatements.DropSeries, name) }
                });
        }

        #endregion Series

        #region Clustering

        public Task<InfluxDbApiResponse> CreateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, User user)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> DeleteClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string name)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> DescribeClusterAdmins(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> UpdateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, User user, string name)
        {
            throw new NotImplementedException();
        }

        #endregion Clustering

        #region Sharding

        public Task<InfluxDbApiResponse> GetShardSpaces(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> DropShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string name)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> CreateShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, ShardSpace shardSpace)
        {
            throw new NotImplementedException();
        }

        #endregion Sharding

        #region Users

        public Task<InfluxDbApiResponse> CreateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, User user)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> DeleteDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string name)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> DescribeDatabaseUsers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> UpdateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, User user, string name)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> AuthenticateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string database, string user, string password)
        {
            throw new NotImplementedException();
        }

        #endregion Users

        #region Other

        /// <summary>Pings the server.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <returns></returns>
        public async Task<InfluxDbApiResponse> Ping(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "ping", null, null, false, true);
        }

        public Task<InfluxDbApiResponse> ForceRaftCompaction(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> Interfaces(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> Sync(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> ListServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers)
        {
            throw new NotImplementedException();
        }

        public Task<InfluxDbApiResponse> RemoveServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>Alters the retention policy.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="policyName">Name of the policy.</param>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="replication">The replication factor.</param>
        /// <returns><see cref="Task{TResult}"/></returns>
        public async Task<InfluxDbApiResponse> AlterRetentionPolicy(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string policyName, string dbName, string duration, int replication)
        {
            return await RequestAsync(errorHandlers, HttpMethod.Get, "query", null,
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
            IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers,
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

            HandleIfErrorResponse(response.StatusCode, responseContent, errorHandlers);

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

        private void HandleIfErrorResponse(HttpStatusCode statusCode, string responseBody, IEnumerable<ApiResponseErrorHandlingDelegate> handlers)
        {
            if (handlers == null)
            {
                throw new ArgumentNullException("handlers");
            }

            foreach (ApiResponseErrorHandlingDelegate handler in handlers)
            {
                handler(statusCode, responseBody);
            }

            _defaultErrorHandlingDelegate(statusCode, responseBody);
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