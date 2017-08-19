using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v0.9.6 Integration")]
    [Trait("InfluxDb v0.9.6 Integration", "Database")]
    public class IntegrationDatabase_v_0_9_6 : IntegrationDatabase
    {
        public IntegrationDatabase_v_0_9_6(IntegrationFixture_v_0_9_6 fixture) : base(fixture)
        {
        }
    }
}
