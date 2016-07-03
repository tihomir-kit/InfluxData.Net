using InfluxData.Net.Kapacitor.ClientModules;
using InfluxData.Net.Kapacitor.RequestClients;

namespace InfluxData.Net.Kapacitor.Infrastructure
{
    /// <summary>
    /// Container class which holds resolved InfluxDbClient dependencies based on 'InfluxDbVersion'.
    /// </summary>
    internal class KapacitorClientDependencies
    {
        public IKapacitorRequestClient KapacitorRequestClient { get; set; }

        public ITaskClientModule TaskClientModule { get; set; }
    }
}
