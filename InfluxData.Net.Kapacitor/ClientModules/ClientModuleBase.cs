using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using InfluxData.Net.Kapacitor.Infrastructure;
using InfluxData.Net.Kapacitor.RequestClients;
using System.Threading.Tasks;
using InfluxData.Net.Common.Helpers;

namespace InfluxData.Net.Kapacitor.ClientModules
{
    public class ClientModuleBase
    {
        protected IKapacitorRequestClient RequestClient { get; private set; }

        public ClientModuleBase(IKapacitorRequestClient requestClient)
        {
            this.RequestClient = requestClient;
        }
    }
}
