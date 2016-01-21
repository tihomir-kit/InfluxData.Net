using Xunit;

namespace InfluxData.Net.Integration.InfluxDb
{
    [CollectionDefinition("InfluxDb Integration")]
    public class IntegrationCollection : ICollectionFixture<IntegrationFixture>
    {
    }
}