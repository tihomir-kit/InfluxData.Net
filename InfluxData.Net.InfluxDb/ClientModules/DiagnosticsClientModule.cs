using System.Net.Http;
using System.Threading.Tasks;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.QueryBuilders;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.ResponseParsers;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class DiagnosticsClientModule : ClientModuleBase, IDiagnosticsClientModule
    {
        private readonly IDiagnosticsQueryBuilder _diagnosticsQueryBuilder;
        private readonly IDiagnosticsResponseParser _diagnosticsResponseParser;

        public DiagnosticsClientModule(IInfluxDbRequestClient requestClient, IDiagnosticsQueryBuilder diagnosticsQueryBuilder, IDiagnosticsResponseParser diagnosticsResponseParser)
            : base(requestClient)
        {
            _diagnosticsQueryBuilder = diagnosticsQueryBuilder;
            _diagnosticsResponseParser = diagnosticsResponseParser;
        }

        public virtual async Task<Pong> PingAsync()
        {
            var watch = Stopwatch.StartNew();
            var response = await RequestClient.RequestAsync(HttpMethod.Get, RequestPaths.Ping, includeAuthToQuery: false, headerIsBody: true).ConfigureAwait(false);

            watch.Stop();

            var pong = new Pong
            {
                Version = response.Body,
                ResponseTime = watch.Elapsed,
                Success = true
            };

            return pong;
        }

        public virtual async Task<Stats> GetStatsAsync()
        {
            var query = _diagnosticsQueryBuilder.GetStats();
            var series = await ResolveSingleGetSeriesResultAsync(query);
            var stats = _diagnosticsResponseParser.GetStats(series);

            return stats;
        }

        public virtual async Task<Diagnostics> GetDiagnosticsAsync()
        {
            var query = _diagnosticsQueryBuilder.GetDiagnostics();
            var series = await ResolveSingleGetSeriesResultAsync(query);
            var diagnostics = _diagnosticsResponseParser.GetDiagnostics(series);

            return diagnostics;
        }
    }
}
