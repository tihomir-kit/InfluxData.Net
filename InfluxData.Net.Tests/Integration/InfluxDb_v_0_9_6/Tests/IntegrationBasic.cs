using System.Threading.Tasks;
using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v0.9.6 Integration")]
    [Trait("InfluxDb v0.9.6 Integration", "Basic")]
    public class IntegrationBasic_v_0_9_6 : IntegrationBasic
    {
        public IntegrationBasic_v_0_9_6(IntegrationFixture_v_0_9_6 fixture) : base(fixture)
        {
        }

        [Fact(Skip = "Test not applicable for this InfluxDB version")]
        public override Task ClientWrite_OnWhitespaceInFieldValue_ShouldNotSaveEscapedWhitespace()
        {
            return null;
        }
    }
}
