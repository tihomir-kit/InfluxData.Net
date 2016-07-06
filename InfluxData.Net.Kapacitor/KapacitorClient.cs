using System;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Kapacitor.ClientModules;
using InfluxData.Net.Kapacitor.Infrastructure;
using InfluxData.Net.Kapacitor.RequestClients;
using System.Net.Http;
using InfluxData.Net.Common.Infrastructure;

namespace InfluxData.Net.Kapacitor
{
    public class KapacitorClient : IKapacitorClient
    {
        private readonly IKapacitorRequestClient _requestClient;

        private readonly Lazy<ITaskClientModule> _taskClientModule;

        public ITaskClientModule Task
        {
            get { return _taskClientModule.Value; }
        }

        public KapacitorClient(string uri, KapacitorVersion kapacitorVersion, HttpClient httpClient = null)
            : this(new KapacitorClientConfiguration(new Uri(uri), null, null, kapacitorVersion, httpClient))
        {
        }

        //public KapacitorClient(string uri, string username, string password, KapacitorVersion kapacitorVersion)
        //     : this(new KapacitorClientConfiguration(new Uri(uri), username, password, kapacitorVersion))
        //{
        //}

        public KapacitorClient(IKapacitorClientConfiguration configuration)
        {
            var requestClientFactory = new KapacitorClientBootstrap(configuration);
            var dependencies = requestClientFactory.GetRequestClient();
            _requestClient = dependencies.KapacitorRequestClient;

            _taskClientModule = new Lazy<ITaskClientModule>(() => dependencies.TaskClientModule);
        }
    }
}