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
        private const string _basePath = "api/v1/";

        public KapacitorRequestClient_v_0_10_0(IKapacitorClientConfiguration configuration)
            : base(configuration)
        {
        }

        protected override string ResolveFullPath(string path)
        {
            return String.Format("{0}{1}", _basePath, path);
        }
    }
}