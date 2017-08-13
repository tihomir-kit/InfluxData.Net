using InfluxData.Net.Common.Enums;

namespace InfluxData.Net.Integration.Kapacitor
{
    public class IntegrationFixture_v_1_0_0 : IntegrationFixtureBase
    {
        public IntegrationFixture_v_1_0_0()
            :base ("InfluxSettings:InfluxDbEndpointUri_v_1_0_0", InfluxDbVersion.v_1_0_0, "InfluxSettings:KapacitorEndpointUri_v_1_0_0", KapacitorVersion.v_1_0_0)
        {
        }
    }
}