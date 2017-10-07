using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v1.3.X Integration")]
    [Trait("InfluxDb v1.3.X Integration", "Continuous Queries")]
    public class IntegrationContinuousQueries_v_1_3 : IntegrationContinuousQueries
    {
        public IntegrationContinuousQueries_v_1_3(IntegrationFixture_v_1_3 fixture) : base(fixture)
        {
        }
    }
}
