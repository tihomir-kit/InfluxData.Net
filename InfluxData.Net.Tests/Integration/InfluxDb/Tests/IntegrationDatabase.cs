using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    public abstract class IntegrationDatabase : IDisposable
    {
        protected readonly IIntegrationFixture _fixture;

        public IntegrationDatabase(IIntegrationFixture fixture)
        {
            _fixture = fixture;
            _fixture.TestSetup();
        }

        public void Dispose()
        {
            _fixture.TestTearDown();
        }

        [Fact]
        public virtual async Task Database_OnCreateAndDrop_ShouldReturnSuccess()
        {
            var dbName = _fixture.CreateRandomDbName();

            var createResponse = await _fixture.Sut.Database.CreateDatabaseAsync(dbName);
            var deleteResponse = await _fixture.Sut.Database.DropDatabaseAsync(dbName);

            createResponse.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }
        
        [Fact]
        public virtual async Task GetDatabase_OnDatabaseExists_ShouldReturnDatabaseCollection()
        {
            var dbName = _fixture.CreateRandomDbName();
            var createResponse = await _fixture.Sut.Database.CreateDatabaseAsync(dbName);
            createResponse.Success.Should().BeTrue();

            var databases = await _fixture.Sut.Database.GetDatabasesAsync();

            databases.Should().NotBeNullOrEmpty();
            databases.Single(p => p.Name.Equals(dbName)).Should().NotBeNull();
        }
    }
}
