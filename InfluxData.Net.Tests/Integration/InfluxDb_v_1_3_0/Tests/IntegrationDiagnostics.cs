using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v1.3.0 Integration")]
    [Trait("InfluxDb v1.3.0 Integration", "Diagnostics")]
    public class IntegrationDiagnostics_v_1_3_0 : IntegrationDiagnostics
    {
        public IntegrationDiagnostics_v_1_3_0(IntegrationFixture_v_1_3_0 fixture) : base(fixture)
        {
        }

    }
}
