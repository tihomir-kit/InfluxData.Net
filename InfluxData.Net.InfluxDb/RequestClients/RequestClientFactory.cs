using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
                case InfluxDbVersion.v096:
                case InfluxDbVersion.v095:
                    return new RequestClient(_configuration);
                case InfluxDbVersion.v092:
                    return new RequestClient_v_0_9_2(_configuration);
                case InfluxDbVersion.v08x:
                    throw new NotImplementedException("InfluxDB v0.8.x is not supported by InfluxData.Net library.");
                default:
                    throw new ArgumentOutOfRangeException("influxDbClientConfiguration", String.Format("Unknown version {0}.", _configuration.InfluxVersion));
            }
        }
    }
}
