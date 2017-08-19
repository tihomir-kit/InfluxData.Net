using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v1.0.0 Integration")]
    [Trait("InfluxDb v1.0.0 Integration", "Database")]
    public class IntegrationDatabase_v_1_0_0 : IntegrationDatabase
    {
        public IntegrationDatabase_v_1_0_0(IntegrationFixture_v_1_0_0 fixture) : base(fixture)
        {
        }
    }
}
