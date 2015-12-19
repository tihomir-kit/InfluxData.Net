using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Formatters;
using InfluxData.Net.Common.Helpers;

namespace InfluxData.Net.InfluxDb.RequestClients
{
    public class RequestClient : IInfluxDbRequestClient
    {
        private const string UserAgent = "InfluxData.Net";

        private readonly IInfluxDbClientConfiguration _configuration;

        public RequestClient(IInfluxDbClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>Pings the server.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <returns></returns>
        public async Task<IInfluxDbApiResponse> PingAsync()
        {
            return await RequestAsync(HttpMethod.Get, RequestPaths.Ping, includeAuthToQuery: false, headerIsBody: true);
        }

        public async Task<IInfluxDbApiResponse> GetQueryAsync(Dictionary<string, string> requestParams)
        {
            return await GetQueryAsync(null, requestParams);
        }

        public async Task<IInfluxDbApiResponse> GetQueryAsync(
            HttpContent content = null,
            Dictionary<string, string> requestParams = null,
            bool includeAuthToQuery = true,
            bool headerIsBody = false)
        {
            return await RequestAsync(HttpMethod.Get, RequestPaths.Query, content, requestParams, includeAuthToQuery, headerIsBody);
        }

        public async Task<IInfluxDbApiResponse> PostDataAsync(Dictionary<string, string> requestParams)
        {
            return await PostDataAsync(null, requestParams);
        }

        public async Task<IInfluxDbApiResponse> PostDataAsync(
            HttpContent content = null,
            Dictionary<string, string> requestParams = null,
            bool includeAuthToQuery = true,
            bool headerIsBody = false)
        {
            return await RequestAsync(HttpMethod.Post, RequestPaths.Write, content, requestParams, includeAuthToQuery, headerIsBody);
        }

        public virtual IInfluxDbFormatter GetFormatter()
        {
            return new Formatter();
        }

        private HttpClient GetHttpClient()
        {
            return new HttpClient();
        }

        private async Task<IInfluxDbApiResponse> RequestAsync(
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
    }
}