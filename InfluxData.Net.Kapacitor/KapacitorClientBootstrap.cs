using System;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Kapacitor.Infrastructure;
using InfluxData.Net.Kapacitor.RequestClients;

namespace InfluxData.Net.Kapacitor
{
    internal class KapacitorClientBootstrap
    {
        private readonly IKapacitorClientConfiguration _configuration;

        public KapacitorClientBootstrap(IKapacitorClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IKapacitorRequestClient GetRequestClient()
        {
            switch (_configuration.KapacitorVersion)
            {
                case KapacitorVersion.Latest:
                case KapacitorVersion.v_1_0_0:
                case KapacitorVersion.v_0_10_1:
                    return new KapacitorRequestClient(_configuration);
                case KapacitorVersion.v_0_10_0:
                    return new KapacitorRequestClient_v_0_10_0(_configuration);
                default:
                    throw new ArgumentOutOfRangeException("kapacitorClientConfiguration", String.Format("Unknown version {0}.", _configuration.KapacitorVersion));
            }
        }
    }
}
