using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.Common.RequestClients;

namespace InfluxData.Net.Kapacitor.RequestClients
{
    public class KapacitorRequestClient : RequestClientBase, IKapacitorRequestClient
    {
        protected virtual string BasePath
        {
            get { return "kapacitor/v1/"; }
        }

        public KapacitorRequestClient(IKapacitorClientConfiguration configuration)
            : base(configuration, "InfluxData.Net.Kapacitor")
        {
        }

        public virtual async Task<IInfluxDataApiResponse> GetAsync(string path)
        {
            return await base.RequestAsync(HttpMethod.Get, ResolveFullPath(path), includeAuthToQuery: false).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> GetAsync(string path, string taskId)
        {
            return await base.RequestAsync(HttpMethod.Get, ResolveFullPath(path, taskId), includeAuthToQuery: false).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> GetAsync(
            string path,
            IDictionary<string, string> requestParams)
        {
            return await base.RequestAsync(HttpMethod.Get, ResolveFullPath(path), requestParams, includeAuthToQuery: false).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> PostAsync(string path, IDictionary<string, string> requestParams = null, string content = null)
        {
            var httpContent = new StringContent(content, Encoding.UTF8, "text/plain");

            return await base.RequestAsync(HttpMethod.Post, ResolveFullPath(path), requestParams, httpContent, false).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> DeleteAsync(string path, string taskId)
        {
            var result = await base.RequestAsync(HttpMethod.Delete, ResolveFullPath(path, taskId), includeAuthToQuery: false).ConfigureAwait(false);

            return new InfluxDataApiDeleteResponse(result.StatusCode, result.Body);
        }

        public virtual async Task<IInfluxDataApiResponse> DeleteAsync(string path, IDictionary<string, string> requestParams = null)
        {
            var result = await base.RequestAsync(HttpMethod.Delete, ResolveFullPath(path), requestParams, includeAuthToQuery: false).ConfigureAwait(false);

            return new InfluxDataApiDeleteResponse(result.StatusCode, result.Body);
        }

        public virtual async Task<IInfluxDataApiResponse> PatchAsync(string path, string taskId, string content = null)
        {
            var httpContent = new StringContent(content, Encoding.UTF8, "text/plain");

            return await base.RequestAsync(new HttpMethod("PATCH"), ResolveFullPath(path, taskId), content: httpContent, includeAuthToQuery: false).ConfigureAwait(false);
        }

        protected virtual string ResolveFullPath(string path, string taskId = null)
        {
            var basePath = String.Format("{0}{1}", this.BasePath, path);

            if (!String.IsNullOrEmpty(taskId))
                return String.Format("{0}/{1}", basePath, taskId);

            return basePath;
        }
    }
}