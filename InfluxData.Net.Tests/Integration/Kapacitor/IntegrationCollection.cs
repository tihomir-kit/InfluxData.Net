using Xunit;

namespace InfluxData.Net.Integration.Kapacitor
{
    [CollectionDefinition("Kapacitor Integration")]
    public class IntegrationCollection : ICollectionFixture<IntegrationFixture>
    {
    }
}