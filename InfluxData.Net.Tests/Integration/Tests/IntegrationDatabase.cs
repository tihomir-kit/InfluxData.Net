using System;
using System.Linq;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace InfluxData.Net.Integration.Tests
{
    [Collection("Integration")]
    [Trait("Integration", "Database")]
    public class IntegrationDatabase : IDisposable
    {
        private readonly IntegrationFixture _fixture;

        public IntegrationDatabase(IntegrationFixture fixture)
        {
            _fixture = fixture;
            _fixture.TestSetup();
        }

        public void Dispose()
        {
            _fixture.TestTearDown();
        }

        [Fact]
        public async Task Database_OnCreateAndDrop_ShouldReturnSuccess()
        {
            // Arrange
            var dbName = _fixture.CreateRandomDbName();

            // Act
            var createResponse = await _fixture.Sut.Database.CreateDatabaseAsync(dbName);
            var deleteResponse = await _fixture.Sut.Database.DropDatabaseAsync(dbName);

            // Assert
            createResponse.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }

        [Fact]
        public async Task DatabaseShow_OnDatabaseExists_ShouldReturnDatabaseList()
        {
            // Arrange
            var dbName = _fixture.CreateRandomDbName();
            var createResponse = await _fixture.Sut.Database.CreateDatabaseAsync(dbName);
            createResponse.Success.Should().BeTrue();

            // Act
            var databases = await _fixture.Sut.Database.GetDatabasesAsync();

            // Assert
            databases
                .Should()
                .NotBeNullOrEmpty();

            databases
                .Where(db => db.Name.Equals(dbName))
                .Single()
                .Should()
                .NotBeNull();
        }
    }
}
