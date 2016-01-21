using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using InfluxData.Net.Kapacitor.Infrastructure;

namespace InfluxData.Net.Kapacitor.RequestClients
{
    public interface IKapacitorRequestClient
    {
        Task<IKapacitorApiResponse> GetDataAsync(string path, IDictionary<string, string> requestParams = null);

        Task<IKapacitorApiResponse> PostDataAsync(string path, IDictionary<string, string> requestParams = null, HttpContent content = null);
    }
}