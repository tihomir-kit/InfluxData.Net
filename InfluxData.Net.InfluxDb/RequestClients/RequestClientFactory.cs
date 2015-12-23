using System;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Infrastructure;

namespace InfluxData.Net.InfluxDb.RequestClients
{
    internal class RequestClientFactory
    {
        private readonly IInfluxDbClientConfiguration _configuration;

        internal RequestClientFactory(IInfluxDbClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        internal IInfluxDbRequestClient GetRequestClient()
        {
            switch (_configuration.InfluxVersion)
            {
                case InfluxDbVersion.Latest:
                case InfluxDbVersion.v_0_9_6:
                case InfluxDbVersion.v_0_9_5:
                    return new RequestClient(_configuration);
                case InfluxDbVersion.v_0_9_2:
                    return new RequestClient_v_0_9_2(_configuration);
                case InfluxDbVersion.v_0_8_x:
                    throw new NotImplementedException("InfluxDB v0.8.x is not supported by InfluxData.Net library.");
                default:
                    throw new ArgumentOutOfRangeException("influxDbClientConfiguration", String.Format("Unknown version {0}.", _configuration.InfluxVersion));
            }
        }
    }
}
