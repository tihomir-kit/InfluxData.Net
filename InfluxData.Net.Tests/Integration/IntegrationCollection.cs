using Xunit;

namespace InfluxData.Net.Integration
{
    [CollectionDefinition("Integration")]
    public class IntegrationCollection : ICollectionFixture<IntegrationFixture>
    {

    }
}