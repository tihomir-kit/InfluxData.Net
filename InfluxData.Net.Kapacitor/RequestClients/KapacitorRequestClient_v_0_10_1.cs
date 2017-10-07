using System;
using InfluxData.Net.Common.Infrastructure;

namespace InfluxData.Net.Kapacitor.RequestClients
{
    public class KapacitorRequestClient_v_0_10_1 : KapacitorRequestClient, IKapacitorRequestClient
    {
        protected override string BasePath
        {
            get { return String.Empty; }
        }

        public KapacitorRequestClient_v_0_10_1(IKapacitorClientConfiguration configuration)
            : base(configuration)
        {
        }
    }
}