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
            switch (_configuration.KapacitorVersion)
            {
                case KapacitorVersion.Latest:
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
