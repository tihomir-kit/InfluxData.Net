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
    public class KapacitorRequestClient_v_0_10_0 : KapacitorRequestClient, IKapacitorRequestClient
    {
        protected override string BasePath
        {
            get { return "api/v1/"; }
        }

        public KapacitorRequestClient_v_0_10_0(IKapacitorClientConfiguration configuration)
            : base(configuration)
        {
        }
    }
}