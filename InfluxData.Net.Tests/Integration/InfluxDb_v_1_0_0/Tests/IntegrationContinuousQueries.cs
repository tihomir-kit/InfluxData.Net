using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v1.0.0 Integration")]
    [Trait("InfluxDb v1.0.0 Integration", "Continuous Queries")]
    public class IntegrationContinuousQueries_v_1_0_0 : IntegrationContinuousQueries
    {
        public IntegrationContinuousQueries_v_1_0_0(IntegrationFixture_v_1_0_0 fixture) : base(fixture)
        {
        }

        [Fact]
        public virtual async Task DeleteContinuousQuery_OnNonExistingCq_ShouldNotThrow()
        {
            var result = await _fixture.Sut.ContinuousQuery.DeleteContinuousQueryAsync(_fixture.DbName, "nonexistingcqname");

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Fact(Skip = "Test not applicable for this InfluxDB version")]
        public override async Task DeleteContinuousQuery_OnNonExistingCq_ShouldThrow()
        {
            return;
        }
    }
}
