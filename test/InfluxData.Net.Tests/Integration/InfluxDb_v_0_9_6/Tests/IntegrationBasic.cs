using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v0.9.6 Integration")]
    [Trait("InfluxDb v0.9.6 Integration", "Basic")]
    public class IntegrationBasic_v_0_9_6 : IntegrationBasic
    {
        public IntegrationBasic_v_0_9_6(IntegrationFixture_v_0_9_6 fixture) : base(fixture)
        {
        }
    }
}
