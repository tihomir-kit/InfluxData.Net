using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.Common.Helpers;

namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    internal class DiagnosticsQueryBuilder : IDiagnosticsQueryBuilder
    {
        public string GetStats()
        {
            return QueryStatements.GetStats;
        }

        public string GetDiagnostics()
        {
            return QueryStatements.GetDiagnostics;
        }
    }
}
