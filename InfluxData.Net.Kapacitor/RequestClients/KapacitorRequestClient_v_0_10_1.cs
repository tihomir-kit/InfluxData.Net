using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.Common.RequestClients;
using InfluxData.Net.Kapacitor.Infrastructure;

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