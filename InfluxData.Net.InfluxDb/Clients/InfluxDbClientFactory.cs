using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.Clients
{
    public class InfluxDbClientFactory
    {
        private readonly InfluxDbClientConfiguration _configuration;

        public InfluxDbClientFactory(InfluxDbClientConfiguration influxDbClientConfiguration)
        {
            _configuration = influxDbClientConfiguration;
        }

        internal IInfluxDbClient GetClient()
        {
            switch (_configuration.InfluxVersion)
            {
                case InfluxDbVersion.Latest:
                    return new InfluxDbClientV09x(_configuration);
                case InfluxDbVersion.v096:
                    return new InfluxDbClientV096(_configuration);
                case InfluxDbVersion.v095:
                    return new InfluxDbClientV095(_configuration);
                case InfluxDbVersion.v092:
                    return new InfluxDbClientV092(_configuration);
                case InfluxDbVersion.v08x:
                    throw new NotImplementedException("InfluxDB v0.8.x is not supported by InfluxData.Net library.");
                default:
                    throw new ArgumentOutOfRangeException("influxDbClientConfiguration", String.Format("Unknown version {0}.", _configuration.InfluxVersion));
            }
        }
    }
}
