using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using InfluxData.Net.InfluxDb.Helpers;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    internal class DiagnosticsParser : IDiagnosticsResponseParser
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
                Runtime = series.Where(p => p.Name == "runtime"),
                Database = series.Where(p => p.Name == "database"),
                QueryExecutor = series.Where(p => p.Name == "queryExecutor"),
                Subscriber = series.Where(p => p.Name == "subscriber"),
                Tsm1Cache = series.Where(p => p.Name == "tsm1_cache"),
                Tsm1Filestore = series.Where(p => p.Name == "tsm1_filestore"),
                Tsm1Wal = series.Where(p => p.Name == "tsm1_wal")
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
                CurrentTime = DateTime.Parse(serie.FirstRecordValueAs<object>("currentTime").ToString()),
                Started = DateTime.Parse(serie.FirstRecordValueAs<object>("started").ToString()),
                Uptime = ParseGoDuration(serie.FirstRecordValueAs<string>("uptime"))
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

        /// <summary>
        /// Parses GoLang duration into a TimeSpan object.
        /// </summary>
        /// <param name="duration">GoLang formatted time duration.</param>
        /// <returns>TimeSpan object.</returns>
        protected virtual TimeSpan ParseGoDuration(string duration)
        {
            try
            {
                int h = -1, m = -1, s = -1, ms = -1;
                Regex regex = new Regex("([0-9\\.]+)(.)");
                var matches = regex.Matches(duration);

                foreach (Match match in matches)
                {
                    var value = match.Groups[1].Value; // Numeric duration value
                    var units = match.Groups[2].Value; // Duration suffix (h/m/s)

                    if (units == "h")
                    {
                        h = int.Parse(value);
                    }
                    else if (units == "m")
                    {
                        m = int.Parse(value);
                    }
                    else if (units == "s")
                    {
                        // Split seconds into s and ms
                        var splitSeconds = value.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                        s = int.Parse(splitSeconds[0]); // Parse the seconds portion

                        if (splitSeconds.Length == 2) // Parse the milliseconds portion
                        {
                            var _ms = splitSeconds[1];
                            if (_ms.Length > 3) _ms = _ms.Substring(0, 3);
                            ms = int.Parse(_ms);
                        }
                    }
                }

                return new TimeSpan(
                    0, h > 0 ? h : 0, 
                    m > 0 ? m : 0, 
                    s > 0 ? s : 0, 
                    ms > 0 ? ms : 0
                );
            }
            catch
            {
                return new TimeSpan(0, 0, -1);
            }
        }
    }
}
