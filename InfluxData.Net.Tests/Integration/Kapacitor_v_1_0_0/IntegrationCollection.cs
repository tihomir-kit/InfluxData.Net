using Xunit;

namespace InfluxData.Net.Integration.Kapacitor
{
    [CollectionDefinition("Kapacitor v1.0.0 Integration")] // for sharing of the fixture instance between multiple test classes
    public class IntegrationCollection_v_1_0_0 : ICollectionFixture<IntegrationFixture_v_1_0_0>
    {
    }
}