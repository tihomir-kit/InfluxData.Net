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
    public class IntegrationFixture_v_1_0_0 : IntegrationFixtureBase
    {
        public IntegrationFixture_v_1_0_0()
            :base ("influxDbEndpointUri_v_1_0_0", InfluxDbVersion.v_1_0_0, "kapacitorEndpointUri_v_1_0_0", KapacitorVersion.v_1_0_0)
        {
        }
    }
}