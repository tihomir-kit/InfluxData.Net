using System.Collections.Generic;
using System.Threading.Tasks;

using InfluxData.Net.Infrastructure.Influx;
using InfluxData.Net.Models;
using InfluxData.Net.Enums;
using System.Net.Http;
using InfluxData.Net.Infrastructure.Formatters;

namespace InfluxData.Net.Infrastructure.Clients
{
    internal interface IInfluxDbClient
    {
        Task<InfluxDbApiResponse> GetQueryAsync(Dictionary<string, string> requestParams);

        Task<InfluxDbApiResponse> GetQueryAsync(HttpContent content = null, Dictionary<string, string> requestParams = null, bool includeAuthToQuery = true, bool headerIsBody = false);

        Task<InfluxDbApiResponse> PostWriteAsync(Dictionary<string, string> requestParams);

        Task<InfluxDbApiResponse> PostWriteAsync(HttpContent content = null, Dictionary<string, string> requestParams = null, bool includeAuthToQuery = true, bool headerIsBody = false);

        // TODO: perhaps move to a separate module
        Task<InfluxDbApiResponse> PingAsync();

        IFormatter GetFormatter();
    }
}