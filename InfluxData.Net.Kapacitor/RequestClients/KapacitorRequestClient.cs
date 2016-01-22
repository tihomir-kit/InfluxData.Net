using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.Common.RequestClients;
using InfluxData.Net.Kapacitor.Infrastructure;

namespace InfluxData.Net.Kapacitor.RequestClients
{
    public class KapacitorRequestClient : RequestClientBase, IKapacitorRequestClient
    {
        private const string BasePath = "api/v1/";

        public KapacitorRequestClient(IKapacitorClientConfiguration configuration)
            : base(configuration.EndpointUri.AbsoluteUri, configuration.Username, configuration.Password, "InfluxData.Net.Kapacitor")
        {
        }

        public virtual async Task<IInfluxDataApiResponse> GetAsync(
            string path,
            IDictionary<string, string> requestParams = null)
        {
            var apiPath = String.Format("{0}{1}", BasePath, path);
            return await base.RequestAsync(HttpMethod.Get, apiPath, requestParams, includeAuthToQuery: false);
        }

        public virtual async Task<IInfluxDataApiResponse> PostAsync(
            string path,
            IDictionary<string, string> requestParams = null,
            string content = null)
        {
            var apiPath = String.Format("{0}{1}", BasePath, path);
            var httpContent = new StringContent(content, Encoding.UTF8, "text/plain");

            return await base.RequestAsync(HttpMethod.Post, apiPath, requestParams, httpContent, false);
        }
    }
}