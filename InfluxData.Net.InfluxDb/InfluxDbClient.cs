using System;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.ClientModules;
using InfluxData.Net.InfluxDb.Formatters;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.QueryBuilders;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.ResponseParsers;

namespace InfluxData.Net.InfluxDb
{
    public class InfluxDbClient : IInfluxDbClient
    {
        private readonly IInfluxDbRequestClient _requestClient;

        private readonly Lazy<ISerieQueryBuilder> _serieQueryBuilder;
        private readonly Lazy<IDatabaseQueryBuilder> _databaseQueryBuilder;
        private readonly Lazy<IRetentionQueryBuilder> _retentionQueryBuilder;
        private readonly Lazy<ICqQueryBuilder> _cqQueryBuilder;
        private readonly Lazy<IDiagnosticsQueryBuilder> _diagnosticsQueryBuilder;

        private readonly Lazy<IBasicResponseParser> _basicResponseParser;
        private readonly Lazy<ISerieResponseParser> _serieResponseParser;
        private readonly Lazy<IDatabaseResponseParser> _databaseResponseParser;
        private readonly Lazy<IRetentionResponseParser> _retentionResponseParser;
        private readonly Lazy<ICqResponseParser> _cqResponseParser;
        private readonly Lazy<IDiagnosticsResponseParser> _diagnosticsResponseParser;

        private readonly Lazy<IBasicClientModule> _basicClientModule;
        public IBasicClientModule Client
        { 
            get { return _basicClientModule.Value; }
        }

        private readonly Lazy<ISerieClientModule> _serieClientModule;
        public ISerieClientModule Serie
        {
            get { return _serieClientModule.Value; }
        }

        private readonly Lazy<IDatabaseClientModule> _databaseClientModule;
        public IDatabaseClientModule Database
        {
            get { return _databaseClientModule.Value; }
        }

        private readonly Lazy<IRetentionClientModule> _retentionClientModule;
        public IRetentionClientModule Retention
        {
            get { return _retentionClientModule.Value; }
        }

        private readonly Lazy<ICqClientModule> _cqClientModule;
        public ICqClientModule ContinuousQuery
        {
            get { return _cqClientModule.Value; }
        }

        private readonly Lazy<IDiagnosticsClientModule> _diagnosticsClientModule;
        public IDiagnosticsClientModule Diagnostics
        {
            get { return _diagnosticsClientModule.Value; }
        }

        public InfluxDbClient(string uri, string username, string password, InfluxDbVersion influxVersion)
             : this(new InfluxDbClientConfiguration(new Uri(uri), username, password, influxVersion))
        {
        }

        public InfluxDbClient(IInfluxDbClientConfiguration configuration)
        {
            var requestClientFactory = new RequestClientFactory(configuration);
            _requestClient = requestClientFactory.GetRequestClient();

            // NOTE: once a breaking change occures, QueryBuilders will need to be resolved with factories
            _serieQueryBuilder = new Lazy<ISerieQueryBuilder>(() => new SerieQueryBuilder(), true);
            _databaseQueryBuilder = new Lazy<IDatabaseQueryBuilder>(() => new DatabaseQueryBuilder(), true);
            _retentionQueryBuilder = new Lazy<IRetentionQueryBuilder>(() => new RetentionQueryBuilder(), true);
            _cqQueryBuilder = new Lazy<ICqQueryBuilder>(() => new CqQueryBuilder(), true);
            _diagnosticsQueryBuilder = new Lazy<IDiagnosticsQueryBuilder>(() => new DiagnosticsQueryBuilder(), true);

            // NOTE: once a breaking change occures, Parsers will need to be resolved with factories
            _basicResponseParser = new Lazy<IBasicResponseParser>(() => new BasicResponseParser(), true);
            _serieResponseParser = new Lazy<ISerieResponseParser>(() => new SerieResponseParser(), true);
            _databaseResponseParser = new Lazy<IDatabaseResponseParser>(() => new DatabaseResponseParser(), true);
            _retentionResponseParser = new Lazy<IRetentionResponseParser>(() => new RetentionResponseParser(), true);
            _cqResponseParser = new Lazy<ICqResponseParser>(() => new CqResponseParser(), true);
            _diagnosticsResponseParser = new Lazy<IDiagnosticsResponseParser>(() => new DiagnosticsParser(), true);

            // NOTE: once a breaking change occures, ClientModules will need to be resolved with factories
            _basicClientModule = new Lazy<IBasicClientModule>(() => new BasicClientModule(_requestClient, _basicResponseParser.Value));
            _serieClientModule = new Lazy<ISerieClientModule>(() => new SerieClientModule(_requestClient, _serieQueryBuilder.Value, _serieResponseParser.Value));
            _databaseClientModule = new Lazy<IDatabaseClientModule>(() => new DatabaseClientModule(_requestClient, _databaseQueryBuilder.Value, _databaseResponseParser.Value));
            _retentionClientModule = new Lazy<IRetentionClientModule>(() => new RetentionClientModule(_requestClient, _retentionQueryBuilder.Value, _retentionResponseParser.Value));
            _cqClientModule = new Lazy<ICqClientModule>(() => new CqClientModule(_requestClient, _cqQueryBuilder.Value, _cqResponseParser.Value));
            _diagnosticsClientModule = new Lazy<IDiagnosticsClientModule>(() => new DiagnosticsClientModule(_requestClient, _diagnosticsQueryBuilder.Value, _diagnosticsResponseParser.Value));
        }

        public IPointFormatter GetPointFormatter()
        {
            return _requestClient.GetPointFormatter();
        }
    }
}