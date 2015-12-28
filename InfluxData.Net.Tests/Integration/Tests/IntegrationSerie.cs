using System;
using System.Linq;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace InfluxData.Net.Integration.Tests
{
    [Collection("Integration")]
    [Trait("Integration", "Serie")]
    public class IntegrationSerie : IDisposable
    {
        private readonly IntegrationFixture _fixture;

        public IntegrationSerie(IntegrationFixture fixture)
        {
            _fixture = fixture;
            _fixture.TestSetup();
        }

        public void Dispose()
        {
            _fixture.TestTearDown();
        }

        [Fact]
        public async Task DropSeries_OnExistingSeries_ShouldDropSeries()
        {
            var points = _fixture.CreateMockPoints(1);
            var writeResponse = await _fixture.Sut.Client.WriteAsync(_fixture.DbName, points);
            writeResponse.Success.Should().BeTrue();

            var expected = _fixture.Sut.GetFormatter().PointToSerie(points.First());
            // query
            await _fixture.Query(expected);

            var deleteSerieResponse = await _fixture.Sut.Serie.DropSeriesAsync(_fixture.DbName, points.First().Name);
            deleteSerieResponse.Success.Should().BeTrue();

            // TODO: try to refetch serie from DB
        }
    }
}
