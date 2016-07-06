using System;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.QueryBuilders;
using InfluxData.Net.InfluxDb.ResponseParsers;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.Common.Infrastructure;

namespace InfluxData.Net.InfluxDb
{
    internal class InfluxDbClientBootstrap
    {
        private readonly IInfluxDbClientConfiguration _configuration;

        public InfluxDbClientBootstrap(IInfluxDbClientConfiguration configuration)
        {
            _configuration = configuration;
        }

        public InfluxDbClientDependencies GetClientDependencies()
        {
            switch (_configuration.InfluxVersion)
            {
                case InfluxDbVersion.Latest:
                case InfluxDbVersion.v_1_0_0:
                    return GetLatestClientDependencies();
                case InfluxDbVersion.v_0_9_6:
                case InfluxDbVersion.v_0_9_5:
                    return GeClientDependencies_v_0_9_6();
                case InfluxDbVersion.v_0_9_2:
                    return GeClientDependencies_v_0_9_2();
                case InfluxDbVersion.v_0_8_x:
                    throw new NotImplementedException("InfluxDB v0.8.x is not supported by InfluxData.Net library.");
                default:
                    throw new ArgumentOutOfRangeException("influxDbClientConfiguration", String.Format("Unknown version {0}.", _configuration.InfluxVersion));
            }
        }

        // NOTE: other dependencies should be added to InfluxDbClientDependencies 
        //       as needed to support older versions of InfluxDB

        private InfluxDbClientDependencies GetLatestClientDependencies()
        {
            return new InfluxDbClientDependencies()
            {
                RequestClient = new InfluxDbRequestClient(_configuration),
                CqQueryBuilder = new CqQueryBuilder(),
                SerieResponseParser = new SerieResponseParser()
            };
        }

        private InfluxDbClientDependencies GeClientDependencies_v_0_9_6()
        {
            return new InfluxDbClientDependencies()
            {
                RequestClient = new InfluxDbRequestClient_v_0_9_6(_configuration),
                CqQueryBuilder = new CqQueryBuilder_v_0_9_6(),
                SerieResponseParser = new SerieResponseParser_v_0_9_6()
            };
        }

        private InfluxDbClientDependencies GeClientDependencies_v_0_9_2()
        {
            return new InfluxDbClientDependencies()
            {
                RequestClient = new InfluxDbRequestClient_v_0_9_2(_configuration),
                CqQueryBuilder = new CqQueryBuilder_v_0_9_6(),
                SerieResponseParser = new SerieResponseParser_v_0_9_6()
            };
        }
    }
}
