using System;
using System.Configuration;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.Kapacitor;
using InfluxData.Net.Kapacitor.Models;

namespace InfluxData.Net.Integration.Kapacitor
{
    public class IntegrationFixture_v_0_10_1 : IntegrationFixtureBase
    {
        public IntegrationFixture_v_0_10_1()
            :base ("influxDbEndpointUri_v_0_9_6", InfluxDbVersion.v_0_9_6, "kapacitorEndpointUri_v_0_10_1", KapacitorVersion.v_0_10_1)
        {
        }
    }
}