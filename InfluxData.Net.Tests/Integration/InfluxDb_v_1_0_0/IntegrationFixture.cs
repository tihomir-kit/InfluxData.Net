using InfluxData.Net.Common.Enums;

namespace InfluxData.Net.Integration.InfluxDb
{
    public class IntegrationFixture_v_1_0_0 : IntegrationFixtureBase
    {
        public IntegrationFixture_v_1_0_0() : base("InfluxSettings:InfluxDbEndpointUri_v_1_0_0", InfluxDbVersion.v_1_0_0)
        {
        }
    }
}