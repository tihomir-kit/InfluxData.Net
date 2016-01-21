using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using InfluxData.Net.Kapacitor.Infrastructure;

namespace InfluxData.Net.Kapacitor.RequestClients
{
    public interface IKapacitorRequestClient
    {
        Task<IKapacitorApiResponse> GetQueryAsync(IDictionary<string, string> requestParams);

        Task<IKapacitorApiResponse> GetQueryAsync(HttpContent content = null, IDictionary<string, string> requestParams = null, bool includeAuthToQuery = true, bool headerIsBody = false);

        Task<IKapacitorApiResponse> PostDataAsync(IDictionary<string, string> requestParams);

        Task<IKapacitorApiResponse> PostDataAsync(HttpContent content = null, IDictionary<string, string> requestParams = null, bool includeAuthToQuery = true, bool headerIsBody = false);
    }
}