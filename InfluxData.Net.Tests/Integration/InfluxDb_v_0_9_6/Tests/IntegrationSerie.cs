using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v0.9.6 Integration")]
    [Trait("InfluxDb v0.9.6 Integration", "Serie")]
    public class IntegrationSerie_v_0_9_6 : IntegrationSerie
    {
        public IntegrationSerie_v_0_9_6(IntegrationFixture_v_0_9_6 fixture) : base(fixture)
        {
        }
    }
}
