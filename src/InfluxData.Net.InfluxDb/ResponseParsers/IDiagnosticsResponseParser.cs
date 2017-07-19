using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    public interface IDiagnosticsResponseParser
    {
        Stats GetStats(IEnumerable<Serie> series);

        Diagnostics GetDiagnostics(IEnumerable<Serie> series);
    }
}
