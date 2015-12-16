using InfluxData.Net.InfluxDb.Formatters;
using InfluxData.Net.InfluxDb.Infrastructure;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace InfluxData.Net.InfluxDb.RequestClients
{
    public interface IInfluxDbRequestClient
    {
        // TODO: perhaps move to a separate module
        Task<InfluxDbApiResponse> PingAsync();

        IInfluxDbFormatter GetFormatter();

        Task<InfluxDbApiResponse> GetQueryAsync(Dictionary<string, string> requestParams);

        Task<InfluxDbApiResponse> GetQueryAsync(HttpContent content = null, Dictionary<string, string> requestParams = null, bool includeAuthToQuery = true, bool headerIsBody = false);

        Task<InfluxDbApiResponse> PostDataAsync(Dictionary<string, string> requestParams);

        Task<InfluxDbApiResponse> PostDataAsync(HttpContent content = null, Dictionary<string, string> requestParams = null, bool includeAuthToQuery = true, bool headerIsBody = false);
    }
}