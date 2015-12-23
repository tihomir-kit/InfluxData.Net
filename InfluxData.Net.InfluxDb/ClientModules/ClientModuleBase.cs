using InfluxData.Net.InfluxDb.RequestClients;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class ClientModuleBase
    {
        protected readonly IInfluxDbRequestClient _requestClient;

        public ClientModuleBase(IInfluxDbRequestClient requestClient)
        {
            _requestClient = requestClient;
        }
    }
}
