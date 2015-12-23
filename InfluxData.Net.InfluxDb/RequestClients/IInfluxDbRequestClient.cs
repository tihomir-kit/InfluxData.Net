using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Formatters;
using InfluxData.Net.InfluxDb.Infrastructure;

namespace InfluxData.Net.InfluxDb.RequestClients
{
    public interface IInfluxDbRequestClient
    {
        IInfluxDbFormatter GetFormatter();

        Task<IInfluxDbApiResponse> PingAsync();

        Task<IInfluxDbApiResponse> GetQueryAsync(Dictionary<string, string> requestParams);

        Task<IInfluxDbApiResponse> GetQueryAsync(HttpContent content = null, Dictionary<string, string> requestParams = null, bool includeAuthToQuery = true, bool headerIsBody = false);

        Task<IInfluxDbApiResponse> PostDataAsync(Dictionary<string, string> requestParams);

        Task<IInfluxDbApiResponse> PostDataAsync(HttpContent content = null, Dictionary<string, string> requestParams = null, bool includeAuthToQuery = true, bool headerIsBody = false);
    }
}