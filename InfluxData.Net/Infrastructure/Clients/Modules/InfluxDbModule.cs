using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.Infrastructure.Clients.Modules
{
    internal class InfluxDbModule
    {
        protected IInfluxDbClient Client { get; private set; }

        public InfluxDbModule(IInfluxDbClient client)
        {
            this.Client = client;
        }
    }
}
