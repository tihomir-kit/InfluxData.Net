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
    }
}
