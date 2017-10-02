using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using InfluxData.Net.Common.Constants;
using InfluxData.Net.Common.Infrastructure;
using System.Diagnostics;
using InfluxData.Net.Common.Helpers;


#if DEBUG

using InfluxData.Net.Common.Helpers;
using System.Diagnostics;

#endif

namespace InfluxData.Net.Common.RequestClients
{
    public abstract class RequestClientBase
    {
        public IConfiguration Configuration { get; private set; }

        private readonly string _userAgent;

        protected RequestClientBase(IInfluxDbClientConfiguration configuration, string userAgent) 
            : this((IConfiguration)configuration, userAgent)
        {
        }

        protected RequestClientBase(IKapacitorClientConfiguration configuration, string userAgent)
            : this((IConfiguration)configuration, userAgent)
        {
        }

        protected RequestClientBase(IConfiguration configuration, string userAgent)
        {
            this.Configuration = configuration;
            _userAgent = userAgent;
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

            return await this.Configuration.HttpClient.SendAsync(request, completionOption, cancellationToken).ConfigureAwait(false);
        }

        #endregion Request Base

        #region Helpers

        private StringBuilder BuildUri(string path, IDictionary<string, string> requestParams, bool includeAuthToQuery)
        {
            var urlBuilder = new StringBuilder();
            urlBuilder.AppendFormat("{0}{1}", this.Configuration.EndpointUri.AbsoluteUri, path);

            if (includeAuthToQuery)
            {
                urlBuilder.AppendFormat("?{0}={1}&{2}={3}", 
                    QueryParams.Username, Uri.EscapeDataString(this.Configuration.Username), 
                    QueryParams.Password, Uri.EscapeDataString(this.Configuration.Password)
                );
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