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
