using InfluxData.Net.Kapacitor.RequestClients;

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
