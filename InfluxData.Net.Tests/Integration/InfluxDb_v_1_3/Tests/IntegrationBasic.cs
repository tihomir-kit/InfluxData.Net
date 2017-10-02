using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v1.3.X Integration")]
    [Trait("InfluxDb v1.3.X Integration", "Basic")]
    public class IntegrationBasic_v_1_3 : IntegrationBasic
    {
        public IntegrationBasic_v_1_3(IntegrationFixture_v_1_3 fixture) : base(fixture)
        {
        }
    }
}
