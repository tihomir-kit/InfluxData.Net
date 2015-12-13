using InfluxData.Net.Client;
using InfluxData.Net.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.Client.Modules
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
