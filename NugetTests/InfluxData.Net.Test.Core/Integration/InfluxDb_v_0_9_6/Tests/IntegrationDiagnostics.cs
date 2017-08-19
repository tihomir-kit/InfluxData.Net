using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v0.9.6 Integration")]
    [Trait("InfluxDb v0.9.6 Integration", "Diagnostics")]
    public class IntegrationDiagnostics_v_0_9_6 : IntegrationDiagnostics
    {
        public IntegrationDiagnostics_v_0_9_6(IntegrationFixture_v_0_9_6 fixture) : base(fixture)
        {
        }

        public override async Task GetStats_ShouldReturnDbStats()
        {
            var stats = await _fixture.Sut.Diagnostics.GetStatsAsync();
            stats.Runtime.Count().Should().BeGreaterThan(0);
            stats.Httpd.Count().Should().BeGreaterThan(0);
            stats.Engine.Count().Should().BeGreaterThan(0);
        }
    }
}
