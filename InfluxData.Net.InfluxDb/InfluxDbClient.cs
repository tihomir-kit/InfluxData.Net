using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.RequestClients.Modules;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Formatters;
using InfluxData.Net.InfluxDb.ClientModules;

namespace InfluxData.Net.InfluxDb
{
    public class InfluxDbClient : IInfluxDbClient
    {
        private readonly IInfluxDbRequestClient _requestClient;
        private readonly Lazy<IBasicRequestModule> _basicRequestModule;
        private readonly Lazy<IDatabaseRequestModule> _databaseRequestModule;
        private readonly Lazy<ICqRequestModule> _cqRequestModule;

        private readonly Lazy<IBasicClientModule> _basicClientModule;
        public IBasicClientModule Client
        { 
            get { return _basicClientModule.Value; }
        }

        private readonly Lazy<IDatabaseClientModule> _databaseClientModule;
        public IDatabaseClientModule Database
        {
            get { return _databaseClientModule.Value; }
        }

        private readonly Lazy<ICqClientModule> _cqClientModule;
        public ICqClientModule ContinuousQuery
        {
            get { return _cqClientModule.Value; }
        }

        public InfluxDbClient(string url, string username, string password, InfluxDbVersion influxVersion)
             : this(new InfluxDbClientConfiguration(new Uri(url), username, password, influxVersion))
        {
        }

        public InfluxDbClient(IInfluxDbClientConfiguration configuration)
        {
            var requestClientFactory = new RequestClientFactory(configuration);
            _requestClient = requestClientFactory.GetRequestClient();

            // NOTE: once a breaking change occures, RequestModules will need to be resolved with factories
            _basicRequestModule = new Lazy<IBasicRequestModule>(() => new BasicRequestModule(_requestClient), true);
            _databaseRequestModule = new Lazy<IDatabaseRequestModule>(() => new DatabaseRequestModule(_requestClient), true);
            _cqRequestModule = new Lazy<ICqRequestModule>(() => new CqRequestModule(_requestClient), true);

            // NOTE: once a breaking change occures, ClientModules will need to be resolved with factories
            _basicClientModule = new Lazy<IBasicClientModule>(() => new BasicClientModule(_requestClient, _basicRequestModule.Value));
            _databaseClientModule = new Lazy<IDatabaseClientModule>(() => new DatabaseClientModule(_requestClient, _databaseRequestModule.Value));
            _cqClientModule = new Lazy<ICqClientModule>(() => new CqClientModule(_requestClient, _cqRequestModule.Value));
        }

        public IInfluxDbFormatter GetFormatter()
        {
            return _requestClient.GetFormatter();
        }
    }
}