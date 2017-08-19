using Xunit;

namespace InfluxData.Net.Integration.Kapacitor.Tests
{
    [Collection("Kapacitor v1.0.0 Integration")] // for sharing of the fixture instance between multiple test classes
    [Trait("Kapacitor v1.0.0 Integration", "Task")] // for organization of tests; by category, subtype
    public class IntegrationTask_v_1_0_0 : IntegrationTask
    {
        public IntegrationTask_v_1_0_0(IntegrationFixture_v_1_0_0 fixture) : base(fixture)
        {
        }
    }
}
