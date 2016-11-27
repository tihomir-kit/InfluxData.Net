using System;

namespace InfluxData.Net.InfluxDb.Models.Responses
{
    public class Diagnostics
    {
        public DiagnosticsSystem System { get; set; }

        public DiagnosticsBuild Build { get; set; }

        public DiagnosticsRuntime Runtime { get; set; }

        public DiagnosticsNetwork Network { get; set; }
    }

    public class DiagnosticsSystem
    {
        public long PID { get; set; }

        public DateTime CurrentTime { get; set; }

        public DateTime Started { get; set; }

        public TimeSpan Uptime { get; set; }
    }

    public class DiagnosticsBuild
    {
        public string Branch { get; set; }

        public string Commit { get; set; }

        public string Version { get; set; }
    }

    public class DiagnosticsRuntime
    {
        public string GOARCH { get; set; }

        public long GOMAXPROCS { get; set; }

        public string GOOS { get; set; }

        public string Version { get; set; }
    }

    public class DiagnosticsNetwork
    {
        public string Hostname { get; set; }
    }
}