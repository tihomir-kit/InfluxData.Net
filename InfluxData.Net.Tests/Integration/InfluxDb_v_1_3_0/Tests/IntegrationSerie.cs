using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v1.3.0 Integration")]
    [Trait("InfluxDb v1.3.0 Integration", "Serie")]
    public class IntegrationSerie_v_1_3_0 : IntegrationSerie
    {
        public IntegrationSerie_v_1_3_0(IntegrationFixture_v_1_3_0 fixture) : base(fixture)
        {
        }
    }
}
