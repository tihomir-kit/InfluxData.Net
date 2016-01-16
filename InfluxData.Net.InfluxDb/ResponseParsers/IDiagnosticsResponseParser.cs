using InfluxData.Net.InfluxDb.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    public interface IDiagnosticsResponseParser
    {
        Stats GetStats(IEnumerable<Serie> series);

        Diagnostics GetDiagnostics(IEnumerable<Serie> series);
    }
}
