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
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.Common.RequestClients;

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
            HttpContent content = null)
        {
            var apiPath = String.Format("{0}{1}", BasePath, path);
            return await base.RequestAsync(HttpMethod.Post, apiPath, requestParams, content, false);
        }
    }
}