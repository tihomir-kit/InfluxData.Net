using InfluxData.Net.InfluxDb.ClientModules;
using InfluxData.Net.InfluxDb.Formatters;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfluxData.Net.InfluxDb
{
    // NOTE: potential "regions/classes": https://docs.influxdata.com/influxdb/v0.9/query_language/

    public interface IInfluxDbClient
    {
        IBasicClientModule Client { get; }

        IDatabaseClientModule Database { get; }

        ICqClientModule ContinuousQuery { get; }

        IInfluxDbFormatter GetFormatter();
    }
}