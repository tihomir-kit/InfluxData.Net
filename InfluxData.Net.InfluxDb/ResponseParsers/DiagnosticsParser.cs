using InfluxData.Net.InfluxDb.Helpers;
using InfluxData.Net.InfluxDb.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    public class DiagnosticsParser : IDiagnosticsResponseParser
    {
        public virtual Stats GetStats(IEnumerable<Serie> series)
        {
            var stats = new Stats()
            {
                CQ = series.Where(p => p.Name == "cq"),
                Engine = series.Where(p => p.Name == "engine"),
                Shard = series.Where(p => p.Name == "shard"),
                Httpd = series.Where(p => p.Name == "httpd"),
                WAL = series.Where(p => p.Name == "wal"),
                Write = series.Where(p => p.Name == "write"),
                Runtime = series.Where(p => p.Name == "runtime")
            };

            return stats;
        }

        public virtual Diagnostics GetDiagnostics(IEnumerable<Serie> series)
        {
            var diagnostics = new Diagnostics()
            {
                System = GetDiagnosticsSystem(series),
                Build = GetDiagnosticsBuild(series),
                Runtime = GetDiagnosticsRuntime(series),
                Network = GetDiagnosticsNetwork(series)
            };

            return diagnostics;
        }

        protected virtual DiagnosticsSystem GetDiagnosticsSystem(IEnumerable<Serie> series)
        {
            var serie = series.GetByName("system");
            var diagnosticsSystem = new DiagnosticsSystem()
            {
                PID = serie.FirstRecordValueAs<long>("PID"),
                CurrentTime = DateTime.Parse(serie.FirstRecordValueAs<string>("currentTime")),
                Started = DateTime.Parse(serie.FirstRecordValueAs<string>("started")),
                Uptime = serie.FirstRecordValueAs<string>("uptime")
            };

            return diagnosticsSystem;
        }

        protected virtual DiagnosticsBuild GetDiagnosticsBuild(IEnumerable<Serie> series)
        {
            var serie = series.GetByName("build");
            var diagnosticsBuild = new DiagnosticsBuild()
            {
                Branch = serie.FirstRecordValueAs<string>("Branch"),
                Commit = serie.FirstRecordValueAs<string>("Commit"),
                Version = serie.FirstRecordValueAs<string>("Version")
            };

            return diagnosticsBuild;
        }

        protected virtual DiagnosticsRuntime GetDiagnosticsRuntime(IEnumerable<Serie> series)
        {
            var serie = series.GetByName("runtime");
            var diagnosticsRuntime = new DiagnosticsRuntime()
            {
                GOARCH = serie.FirstRecordValueAs<string>("GOARCH"),
                GOMAXPROCS = serie.FirstRecordValueAs<long>("GOMAXPROCS"),
                GOOS = serie.FirstRecordValueAs<string>("GOOS"),
                Version = serie.FirstRecordValueAs<string>("version")
            };

            return diagnosticsRuntime;
        }

        protected virtual DiagnosticsNetwork GetDiagnosticsNetwork(IEnumerable<Serie> series)
        {
            var serie = series.GetByName("network");
            var diagnosticsNetwork = new DiagnosticsNetwork()
            {
                Hostname = serie.FirstRecordValueAs<string>("hostname")
            };

            return diagnosticsNetwork;
        }
    }
}
