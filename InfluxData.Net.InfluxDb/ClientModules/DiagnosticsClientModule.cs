using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.QueryBuilders;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class DiagnosticsClientModule : ClientModuleBase, IDiagnosticsClientModule
    {
        private readonly IDiagnosticsQueryBuilder _diagnosticsQueryBuilder;

        public DiagnosticsClientModule(IInfluxDbRequestClient requestClient, IDiagnosticsQueryBuilder diagnosticsQueryBuilder)
            : base(requestClient)
        {
            _diagnosticsQueryBuilder = diagnosticsQueryBuilder;
        }

        public async Task<Pong> PingAsync()
        {
            var watch = Stopwatch.StartNew();
            var response = await this.RequestClient.PingAsync();

            watch.Stop();

            var pong = new Pong
            {
                Version = response.Body,
                ResponseTime = watch.Elapsed,
                Success = true
            };

            return pong;
        }

        public async Task<Stats> GetStats(string dbName)
        {
            throw new NotImplementedException();
        }

        public async Task<Diagnostics> GetDiagnostics()
        {
            throw new NotImplementedException();
        }
    }
}
