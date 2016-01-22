using Xunit;

namespace InfluxData.Net.Integration.Kapacitor
{
    [CollectionDefinition("Kapacitor Integration")] // for sharing of the fixture instance between multiple test classes
    public class IntegrationCollection : ICollectionFixture<IntegrationFixture>
    {
    }
}