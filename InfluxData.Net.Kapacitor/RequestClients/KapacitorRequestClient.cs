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
        public KapacitorRequestClient(IKapacitorClientConfiguration configuration)
            : base(configuration.EndpointUri.AbsoluteUri, configuration.Username, configuration.Password, "InfluxData.Net.Kapacitor")
        {
        }

        public virtual async Task<IInfluxDataApiResponse> GetAsync(
            string path,
            IDictionary<string, string> requestParams = null)
        {
            return await base.RequestAsync(HttpMethod.Get, ResolveFullPath(path), requestParams, includeAuthToQuery: false).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> PostAsync(string path, IDictionary<string, string> requestParams = null, string content = null)
        {
            var httpContent = new StringContent(content, Encoding.UTF8, "text/plain");

            return await base.RequestAsync(HttpMethod.Post, ResolveFullPath(path), requestParams, httpContent, false).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> DeleteAsync(string path, IDictionary<string, string> requestParams = null)
        {
            return await base.RequestAsync(HttpMethod.Delete, ResolveFullPath(path), requestParams, includeAuthToQuery: false).ConfigureAwait(false);
        }

        protected virtual string ResolveFullPath(string path)
        {
            return path; // v_0_10_0 uses different base path
        }
    }
}