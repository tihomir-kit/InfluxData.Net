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
        public async Task Influx_OnFakeDbName_ShouldCreateAndDropDb()
        {
            // Arrange
            var dbName = _fixture.CreateRandomDbName();

            // Act
            var createResponse = await _fixture.Sut.CreateDatabaseAsync(dbName);
            var deleteResponse = await _fixture.Sut.DropDatabaseAsync(dbName);

            // Assert
            createResponse.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }

        [Fact]
        public async Task DbShowDatabases_OnDatabaseExists_ShouldReturnDatabaseList()
        {
            // Arrange
            var dbName = _fixture.CreateRandomDbName();
            var createResponse = await _fixture.Sut.CreateDatabaseAsync(dbName);
            createResponse.Success.Should().BeTrue();

            // Act
            var databases = await _fixture.Sut.ShowDatabasesAsync();

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

        [Fact]
        public async Task DbDropSeries_OnExistingSeries_ShouldDropSeries()
        {
            var points = _fixture.CreateMockPoints(1);
            var writeResponse = await _fixture.Sut.WriteAsync(_fixture.DbName, points);
            writeResponse.Success.Should().BeTrue();

            var expected = _fixture.Sut.GetFormatter().PointToSerie(points.First());
            // query
            await _fixture.Query(expected);

            var deleteSerieResponse = await _fixture.Sut.DropSeriesAsync(_fixture.DbName, points.First().Name);
            deleteSerieResponse.Success.Should().BeTrue();
        }
    }
}
