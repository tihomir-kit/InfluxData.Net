using Xunit;

namespace InfluxData.Net.Integration.Kapacitor
{
    [CollectionDefinition("Kapacitor v0.10.1 Integration")] // for sharing of the fixture instance between multiple test classes
    public class IntegrationCollection_v_0_10_1 : ICollectionFixture<IntegrationFixture_v_0_10_1>
    {
    }
}