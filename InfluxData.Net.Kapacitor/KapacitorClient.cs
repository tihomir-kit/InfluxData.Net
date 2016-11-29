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
        private IKapacitorRequestClient _requestClient;

        private Lazy<ITaskClientModule> _taskClientModule;

        public ITaskClientModule Task
        {
            get { return _taskClientModule.Value; }
        }

        public KapacitorClient(string uri, KapacitorVersion kapacitorVersion, HttpClient httpClient = null, bool throwOnWarning = false)
            : this(new KapacitorClientConfiguration(new Uri(uri), null, null, kapacitorVersion, httpClient, throwOnWarning))
        {
        }

        public KapacitorClient(IKapacitorClientConfiguration configuration)
        {
            switch (configuration.KapacitorVersion)
            {
                case KapacitorVersion.Latest:
                case KapacitorVersion.v_1_0_0:
                    this.BootstrapKapacitorLatest(configuration);
                    break;
                case KapacitorVersion.v_0_10_1:
                    this.BootstrapKapacitorLatest(configuration);
                    this.BootstrapKapacitorLatest_v_0_10_1(configuration);
                    break;
                case KapacitorVersion.v_0_10_0:
                    this.BootstrapKapacitorLatest(configuration);
                    this.BootstrapKapacitorLatest_v_0_10_1(configuration);
                    this.BootstrapKapacitorLatest_v_0_10_0(configuration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("kapacitorClientConfiguration", String.Format("Unknown version {0}.", configuration.KapacitorVersion));
            }
        }

        protected virtual void BootstrapKapacitorLatest(IKapacitorClientConfiguration configuration)
        {
            _requestClient = new KapacitorRequestClient(configuration);

            _taskClientModule = new Lazy<ITaskClientModule>(() => new TaskClientModule(_requestClient));
        }

        protected virtual void BootstrapKapacitorLatest_v_0_10_1(IKapacitorClientConfiguration configuration)
        {
            _requestClient = new KapacitorRequestClient_v_0_10_1(configuration);

            _taskClientModule = new Lazy<ITaskClientModule>(() => new TaskClientModule_v_0_10_1(_requestClient));
        }

        protected virtual void BootstrapKapacitorLatest_v_0_10_0(IKapacitorClientConfiguration configuration)
        {
            _requestClient = new KapacitorRequestClient_v_0_10_0(configuration);
        }
    }
}