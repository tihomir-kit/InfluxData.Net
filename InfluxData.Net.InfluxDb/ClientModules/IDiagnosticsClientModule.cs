using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public interface IDiagnosticsClientModule
    {
        /// <summary>
        /// Pings the InfluxDb server.
        /// </summary>
        /// <returns>Response of the ping execution (success, dbVersion, response time).</returns>
        Task<Pong> PingAsync();

        Task<Stats> GetStats(string dbName);

        Task<Diagnostics> GetDiagnostics();
    }
}