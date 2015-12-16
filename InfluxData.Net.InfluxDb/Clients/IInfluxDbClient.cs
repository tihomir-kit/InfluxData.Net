using InfluxData.Net.InfluxDb.Formatters;
using InfluxData.Net.InfluxDb.Infrastructure;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace InfluxData.Net.InfluxDb.Clients
{
    internal interface IInfluxDbClient
    {
        Task<InfluxDbApiResponse> GetQueryAsync(Dictionary<string, string> requestParams);

        Task<InfluxDbApiResponse> GetQueryAsync(HttpContent content = null, Dictionary<string, string> requestParams = null, bool includeAuthToQuery = true, bool headerIsBody = false);

        Task<InfluxDbApiResponse> PostWriteAsync(Dictionary<string, string> requestParams);

        Task<InfluxDbApiResponse> PostWriteAsync(HttpContent content = null, Dictionary<string, string> requestParams = null, bool includeAuthToQuery = true, bool headerIsBody = false);

        // TODO: perhaps move to a separate module
        Task<InfluxDbApiResponse> PingAsync();

        IInfluxDbFormatter GetFormatter();
    }
}