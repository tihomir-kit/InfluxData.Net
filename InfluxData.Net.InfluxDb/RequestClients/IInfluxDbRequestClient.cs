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
        Task<IInfluxDbApiResponse> PingAsync();

        IInfluxDbFormatter GetFormatter();

        Task<IInfluxDbApiResponse> GetQueryAsync(Dictionary<string, string> requestParams);

        Task<IInfluxDbApiResponse> GetQueryAsync(HttpContent content = null, Dictionary<string, string> requestParams = null, bool includeAuthToQuery = true, bool headerIsBody = false);

        Task<IInfluxDbApiResponse> PostDataAsync(Dictionary<string, string> requestParams);

        Task<IInfluxDbApiResponse> PostDataAsync(HttpContent content = null, Dictionary<string, string> requestParams = null, bool includeAuthToQuery = true, bool headerIsBody = false);
    }
}