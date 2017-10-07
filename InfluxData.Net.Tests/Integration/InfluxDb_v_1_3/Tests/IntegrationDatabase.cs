using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v1.3.X Integration")]
    [Trait("InfluxDb v1.3.X Integration", "Database")]
    public class IntegrationDatabase_v_1_3 : IntegrationDatabase
    {
        public IntegrationDatabase_v_1_3(IntegrationFixture_v_1_3 fixture) : base(fixture)
        {
        }
    }
}
