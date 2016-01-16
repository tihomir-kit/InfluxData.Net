using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using System;

namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    public interface IDiagnosticsQueryBuilder
    {
        string GetStats();

        string GetDiagnostics();
    }
}