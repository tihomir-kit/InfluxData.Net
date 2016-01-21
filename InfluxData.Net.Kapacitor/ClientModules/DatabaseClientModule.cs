using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxData.Net.Kapacitor.Infrastructure;
using InfluxData.Net.Kapacitor.RequestClients;
using System;
using InfluxData.Net.Kapacitor.QueryBuilders;

namespace InfluxData.Net.Kapacitor.ClientModules
{
    public class DatabaseClientModule : ClientModuleBase, IDatabaseClientModule
    {
        public DatabaseClientModule(IKapacitorRequestClient requestClient)
            : base(requestClient)
        {
        }
    }
}
