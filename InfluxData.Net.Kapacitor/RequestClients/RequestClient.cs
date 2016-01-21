using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Kapacitor.Constants;
using InfluxData.Net.Kapacitor.Infrastructure;
using System.Diagnostics;

namespace InfluxData.Net.Kapacitor.RequestClients
{
    public class RequestClient : IKapacitorRequestClient
    {
        private const string UserAgent = "InfluxData.Net.Kapacitor";
        private const string BasePath = "api/v1/";

        private readonly IKapacitorClientConfiguration _configuration;

        public RequestClient(IKapacitorClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Requests

        public virtual async Task<IKapacitorApiResponse> GetDataAsync(
            string path,
            IDictionary<string, string> requestParams = null)
        {
            var apiPath = String.Format("{0}{1}", BasePath, path);
            return await RequestAsync(HttpMethod.Get, apiPath, requestParams: requestParams, includeAuthToQuery: false);
        }

        public virtual async Task<IKapacitorApiResponse> PostDataAsync(
            string path,
            IDictionary<string, string> requestParams = null,
            HttpContent content = null)
        {
            var apiPath = String.Format("{0}{1}", BasePath, path);
            return await RequestAsync(HttpMethod.Post, apiPath, content, requestParams, false);
        }

        #endregion Requests

        #region Request Base

        private async Task<IKapacitorApiResponse> RequestAsync(
            HttpMethod method,
            string path,
            HttpContent content = null,
            IDictionary<string, string> requestParams = null,
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

            return new KapacitorApiResponse(response.StatusCode, responseContent);
        }

        private async Task<HttpResponseMessage> RequestInnerAsync(
            TimeSpan? requestTimeout,
            HttpCompletionOption completionOption,
            CancellationToken cancellationToken,
            HttpMethod method,
            string path,
            HttpContent content = null,
            IDictionary<string, string> extraParams = null,
            bool includeAuthToQuery = true)
        {
            var client = new HttpClient();

            if (requestTimeout.HasValue)
            {
                client.Timeout = requestTimeout.Value;
            }

            var uri = BuildUri(path, extraParams, includeAuthToQuery);
            var request = BuildRequest(method, content, uri);

#if DEBUG
            Debug.WriteLine("[Request] {0}", request.ToJson());
            if (content != null)
            {
                Debug.WriteLine("[RequestData] {0}", content.ReadAsStringAsync().Result);
            }
#endif

            return await client.SendAsync(request, completionOption, cancellationToken);
        }

        #endregion Request Base

        #region Helpers

        private StringBuilder BuildUri(string path, IDictionary<string, string> requestParams, bool includeAuthToQuery)
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.AppendFormat("{0}{1}", _configuration.EndpointUri, path);

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
                throw new KapacitorApiException(statusCode, responseBody);
            }
        }

        #endregion Helpers
    }
}