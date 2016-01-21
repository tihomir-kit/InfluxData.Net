using System;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Kapacitor.Infrastructure;

namespace InfluxData.Net.Kapacitor
{
    public class KapacitorClient : IKapacitorClient
    {
        public KapacitorClient(string uri, string username, string password, InfluxDbVersion influxVersion)
             : this(new KapacitorClientConfiguration(new Uri(uri), username, password, influxVersion))
        {
        }

        public KapacitorClient(KapacitorClientConfiguration configuration)
        {

        }
    }
}