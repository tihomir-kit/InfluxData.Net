using System;
using System.Linq;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb Integration")]
    [Trait("InfluxDb Integration", "Serie")]
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
        public async Task GetSeries_OnNoSeries_ShouldReturnEmptyCollection()
        {
            var dbName = _fixture.CreateRandomDbName();
            await _fixture.CreateEmptyDatabase(dbName);

            var result = await _fixture.Sut.Serie.GetSeriesAsync(dbName);
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetSeries_OnExistingSeries_ShouldReturnSerieSetCollection()
        {
            var dbName = _fixture.CreateRandomDbName();
            await _fixture.CreateEmptyDatabase(dbName);

            var points = await _fixture.MockAndWritePoints(3, 2, dbName);

            var result = await _fixture.Sut.Serie.GetSeriesAsync(dbName);
            result.Should().HaveCount(2);
            var firstSet = result.FirstOrDefault(p => p.Name == points.First().Name);
            firstSet.Should().NotBeNull();
            firstSet.Series.Should().HaveCount(3);
            firstSet.Series.First().Key.Should().NotBeNullOrEmpty();
            firstSet.Series.First().Tags.Should().HaveCount(points.First().Tags.Count);
            var lastSet = result.FirstOrDefault(p => p.Name == points.Last().Name);
            lastSet.Should().NotBeNull();
            lastSet.Series.Should().HaveCount(3);
            lastSet.Series.First().Key.Should().NotBeNullOrEmpty();
            lastSet.Series.First().Tags.Should().HaveCount(points.First().Tags.Count);
        }


        // NOTE: this test is currently useles because of this:
        // https://github.com/influxdata/influxdb/issues/3087#issuecomment-170290120
        //[Fact]
        //public async Task DropSeries_OnExistingSeries_ShouldDropSeries()
        //{
        //    var points = await _fixture.MockAndWritePoints(1);
        //    await _fixture.EnsurePointExists(points.First());

        //    var result = await _fixture.Sut.Serie.DropSeriesAsync(_fixture.DbName, points.First().Name);
        //    result.Success.Should().BeTrue();

        //    var query = String.Format("select * from {0}", points.First().Name);
        //    var series = await _fixture.Sut.Client.QueryAsync(_fixture.DbName, query);
        //    series.Should().HaveCount(0);

        //    var serieSets = await _fixture.Sut.Serie.GetSeriesAsync(_fixture.DbName);
        //    serieSets.FirstOrDefault(p => p.Name == points.First().Name).Should().BeNull();
        //}

        [Fact]
        public async Task GetMeasurements_OnNoSeries_ShouldReturnEmptyCollection()
        {
            var dbName = _fixture.CreateRandomDbName();
            await _fixture.CreateEmptyDatabase(dbName);

            var result = await _fixture.Sut.Serie.GetMeasurementsAsync(dbName);
            result.Should().HaveCount(0);
        }

        [Fact]
        public async Task GetMeasurements_OnExistingSeries_ShouldReturnMeasurementCollection()
        {
            var dbName = _fixture.CreateRandomDbName();
            await _fixture.CreateEmptyDatabase(dbName);
            var points = await _fixture.MockAndWritePoints(2, 2, dbName);

            var result = await _fixture.Sut.Serie.GetMeasurementsAsync(dbName);
            result.Should().HaveCount(2);
            result.FirstOrDefault(p => p.Name == points.First().Name).Name.Should().NotBeNull();
            result.FirstOrDefault(p => p.Name == points.Last().Name).Name.Should().NotBeNull();
        }
    }
}
