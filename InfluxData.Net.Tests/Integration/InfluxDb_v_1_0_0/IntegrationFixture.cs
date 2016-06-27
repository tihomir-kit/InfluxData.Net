using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.Integration.Kapacitor;
using Ploeh.AutoFixture;
using InfluxData.Net.Common.Enums;

namespace InfluxData.Net.Integration.InfluxDb
{
    public class IntegrationFixture_v_1_0_0 : IntegrationFixtureBase
    {
        public IntegrationFixture_v_1_0_0() 
            : base("influxDbEndpointUri_v_1_0_0", InfluxDbVersion.v_1_0_0)
        {
        }
    }
}