using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v1.3.X Integration")]
    [Trait("InfluxDb v1.3.X Integration", "Serie")]
    public class IntegrationSerie_v_1_3 : IntegrationSerie
    {
        public IntegrationSerie_v_1_3(IntegrationFixture_v_1_3 fixture) : base(fixture)
        {
        }
    }
}
