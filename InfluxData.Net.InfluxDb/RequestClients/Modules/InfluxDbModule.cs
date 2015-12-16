using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.RequestClients.Modules
{
    internal class InfluxDbModule
    {
        protected IInfluxDbRequestClient Client { get; private set; }

        public InfluxDbModule(IInfluxDbRequestClient client)
        {
            this.Client = client;
        }
    }
}
