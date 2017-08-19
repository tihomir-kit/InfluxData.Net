using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v1.3.0 Integration")]
    [Trait("InfluxDb v1.3.0 Integration", "Basic")]
    public class IntegrationBasic_v_1_3_0 : IntegrationBasic
    {
        public IntegrationBasic_v_1_3_0(IntegrationFixture_v_1_3_0 fixture) : base(fixture)
        {
        }
    }
}
