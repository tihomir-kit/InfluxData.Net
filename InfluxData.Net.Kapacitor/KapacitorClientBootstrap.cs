using System;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Kapacitor.Infrastructure;
using InfluxData.Net.Kapacitor.RequestClients;
using InfluxData.Net.Kapacitor.ClientModules;
using InfluxData.Net.Common.Infrastructure;

namespace InfluxData.Net.Kapacitor
{
    internal class KapacitorClientBootstrap
    {
        private readonly IKapacitorClientConfiguration _configuration;

        public KapacitorClientBootstrap(IKapacitorClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public KapacitorClientDependencies GetRequestClient()
        {
            switch (_configuration.KapacitorVersion)
            {
                case KapacitorVersion.Latest:
                case KapacitorVersion.v_1_0_0:
                    return GetLatestClientDependencies();
                case KapacitorVersion.v_0_10_1:
                    return GetLatestClientDependencies_v_0_10_1();
                case KapacitorVersion.v_0_10_0:
                    return GetLatestClientDependencies_v_0_10_0();
                default:
                    throw new ArgumentOutOfRangeException("kapacitorClientConfiguration", String.Format("Unknown version {0}.", _configuration.KapacitorVersion));
            }
        }

        // NOTE: other dependencies should be added to KapacitorClientDependencies 
        //       as needed to support older versions of Kapacitor

        private KapacitorClientDependencies GetLatestClientDependencies()
        {
            var kapacitorRequestClient = new KapacitorRequestClient(_configuration);

            return new KapacitorClientDependencies()
            {
                KapacitorRequestClient = kapacitorRequestClient,
                TaskClientModule = new TaskClientModule(kapacitorRequestClient)
            };
        }

        private KapacitorClientDependencies GetLatestClientDependencies_v_0_10_1()
        {
            var kapacitorRequestClient = new KapacitorRequestClient_v_0_10_1(_configuration);

            return new KapacitorClientDependencies()
            {
                KapacitorRequestClient = kapacitorRequestClient,
                TaskClientModule = new TaskClientModule_v_0_10_1(kapacitorRequestClient)
            };
        }

        private KapacitorClientDependencies GetLatestClientDependencies_v_0_10_0()
        {
            var kapacitorRequestClient = new KapacitorRequestClient_v_0_10_0(_configuration);

            return new KapacitorClientDependencies()
            {
                KapacitorRequestClient = kapacitorRequestClient,
                TaskClientModule = new TaskClientModule_v_0_10_1(kapacitorRequestClient)
            };
        }
    }
}
