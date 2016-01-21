using System;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Kapacitor.Infrastructure;

namespace InfluxData.Net.Kapacitor.RequestClients
{
    internal class RequestClientFactory
    {
        private readonly IKapacitorClientConfiguration _configuration;

        public RequestClientFactory(IKapacitorClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IKapacitorRequestClient GetRequestClient()
        {
            switch (_configuration.InfluxVersion)
            {
                case InfluxDbVersion.Latest:
                case InfluxDbVersion.v_0_9_6:
                case InfluxDbVersion.v_0_9_5:
                    return new RequestClient(_configuration);
                case InfluxDbVersion.v_0_8_x:
                    throw new NotImplementedException("InfluxDB v0.8.x is not supported by InfluxData.Net library.");
                default:
                    throw new ArgumentOutOfRangeException("influxDbClientConfiguration", String.Format("Unknown version {0}.", _configuration.InfluxVersion));
            }
        }
    }
}
