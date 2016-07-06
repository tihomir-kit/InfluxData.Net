using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using InfluxData.Net.Common.Helpers;
using System.Diagnostics;
using System.Net.Http;
using InfluxData.Net.Common.Constants;
using InfluxData.Net.Common.Infrastructure;

namespace InfluxData.Net.Common.RequestClients
{
    public abstract class RequestClientBase
    {
        private readonly string _endpointUri;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _userAgent;
        private readonly HttpClient _httpClient;

        protected RequestClientBase(IInfluxDbClientConfiguration configuration, string userAgent) 
            : this(configuration.EndpointUri.AbsoluteUri, configuration.Username, configuration.Password, configuration.HttpClient, userAgent)
        {
        }

        protected RequestClientBase(IKapacitorClientConfiguration configuration, string userAgent)
            : this(configuration.EndpointUri.AbsoluteUri, configuration.Username, configuration.Password, configuration.HttpClient, userAgent)
        {
        }

        protected RequestClientBase(string endpointUri, string userName, string password, HttpClient httpClient, string userAgent)
        {
            _endpointUri = endpointUri;
            _userName = userName;
            _password = password;
            _userAgent = userAgent;
            _httpClient = httpClient ?? new HttpClient();
        }

        #region Request Base

        public async Task<IInfluxDataApiResponse> RequestAsync(
            HttpMethod method,
            string path,
            IDictionary<string, string> requestParams = null,
            HttpContent content = null,
            bool includeAuthToQuery = true,
            bool headerIsBody = false)
        {
            var response = await RequestInnerAsync(
                HttpCompletionOption.ResponseHeadersRead,
                CancellationToken.None,
                method,
                path,
                requestParams,
                content,
                includeAuthToQuery).ConfigureAwait(false);

            string responseContent = String.Empty;

            if (!headerIsBody)
            {
                responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            else
            {
                IEnumerable<string> values;

                if (response.Headers.TryGetValues("X-Influxdb-Version", out values))
                {
                    responseContent = values.First();
                }
                else if (response.Headers.TryGetValues("X-Kapacitor-Version", out values))
                {
                    responseContent = values.First();
                }
            }

            HandleIfErrorResponse(response.StatusCode, responseContent);

#if DEBUG
            Debug.WriteLine("[Response] {0}", response.ToJson());
            Debug.WriteLine("[ResponseData] {0}", responseContent);
#endif

            return new InfluxDataApiResponse(response.StatusCode, responseContent);
        }

        private async Task<HttpResponseMessage> RequestInnerAsync(
            HttpCompletionOption completionOption,
            CancellationToken cancellationToken,
            HttpMethod method,
            string path,
            IDictionary<string, string> extraParams = null,
            HttpContent content = null,
            bool includeAuthToQuery = true)
        {
            var uri = BuildUri(path, extraParams, includeAuthToQuery);
            var request = BuildRequest(method, content, uri);

#if DEBUG
            Debug.WriteLine("[Request] {0}", request.ToJson());
            if (content != null)
            {
                Debug.WriteLine("[RequestData] {0}", content.ReadAsStringAsync().Result);
            }
#endif

            return await _httpClient.SendAsync(request, completionOption, cancellationToken).ConfigureAwait(false);
        }

        #endregion Request Base

        #region Helpers

        private StringBuilder BuildUri(string path, IDictionary<string, string> requestParams, bool includeAuthToQuery)
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.AppendFormat("{0}{1}", _endpointUri, path);

            if (includeAuthToQuery)
            {
                urlBuilder.AppendFormat("?{0}={1}&{2}={3}", QueryParams.Username, HttpUtility.UrlEncode(_userName), QueryParams.Password, HttpUtility.UrlEncode(_password));
            }

            if (requestParams != null && requestParams.Count > 0)
            {
                var keyValues = new List<string>(requestParams.Count);
                keyValues.AddRange(requestParams.Select(param => String.Format("{0}={1}", param.Key, param.Value)));
                urlBuilder.AppendFormat("{0}{1}", includeAuthToQuery ? "&" : "?", String.Join("&", keyValues));
            }

            return urlBuilder;
        }

        private HttpRequestMessage BuildRequest(HttpMethod method, HttpContent content, StringBuilder urlBuilder)
        {
            var request = new HttpRequestMessage(method, urlBuilder.ToString());
            request.Headers.Add("User-Agent", _userAgent);
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
                throw new InfluxDataApiException(statusCode, responseBody);
            }
        }

        #endregion Helpers
    }
}