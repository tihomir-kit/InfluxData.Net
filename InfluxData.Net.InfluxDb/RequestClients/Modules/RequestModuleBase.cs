using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.RequestClients.Modules
{
    internal class RequestModuleBase
    {
        protected IInfluxDbRequestClient RequestClient { get; private set; }

        public RequestModuleBase(IInfluxDbRequestClient requestClient)
        {
            this.RequestClient = requestClient;
        }
    }
}
