using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.Kapacitor.Infrastructure;

namespace InfluxData.Net.Kapacitor.RequestClients
{
    public interface IKapacitorRequestClient
    {
        Task<IInfluxDataApiResponse> GetAsync(string path, IDictionary<string, string> requestParams = null);

        Task<IInfluxDataApiResponse> PostAsync(string path, IDictionary<string, string> requestParams = null, HttpContent content = null);

        Task<IInfluxDataApiResponse> RequestAsync(
            HttpMethod method,
            string path,
            IDictionary<string, string> requestParams = null,
            HttpContent content = null,
            bool includeAuthToQuery = true,
            bool headerIsBody = false);
    }
}