using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    public abstract class IntegrationSerie : IDisposable
    {
        protected readonly IIntegrationFixture _fixture;

        public IntegrationSerie(IIntegrationFixture fixture)
        {
            _fixture = fixture;
            _fixture.TestSetup();
        }

        public void Dispose()
        {
            _fixture.TestTearDown();
        }

        [Fact]
        public virtual async Task GetSeries_OnNoSeries_ShouldReturnEmptyCollection()
        {
            var dbName = _fixture.CreateRandomDbName();
            await _fixture.CreateEmptyDatabase(dbName);

            var result = await _fixture.Sut.Serie.GetSeriesAsync(dbName);
            result.Should().HaveCount(0);
        }

        [Fact]
        public virtual async Task GetSeries_OnExistingSeries_ShouldReturnSerieSetCollection()
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
            // NOTE: currently InfluxDB is not returning any tags, not sure if bug or by design
            //firstSet.Series.First().Tags.Should().HaveCount(points.First().Tags.Count);
            var lastSet = result.FirstOrDefault(p => p.Name == points.Last().Name);
            lastSet.Should().NotBeNull();
            lastSet.Series.Should().HaveCount(3);
            lastSet.Series.First().Key.Should().NotBeNullOrEmpty();
            // NOTE: currently InfluxDB is not returning any tags, not sure if bug or by design
            //lastSet.Series.First().Tags.Should().HaveCount(points.First().Tags.Count);
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
        //    var series = await _fixture.Sut.Client.QueryAsync(query, _fixture.DbName);
        //    series.Should().HaveCount(0);

        //    var serieSets = await _fixture.Sut.Serie.GetSeriesAsync(_fixture.DbName);
        //    serieSets.FirstOrDefault(p => p.Name == points.First().Name).Should().BeNull();
        //}

        [Fact]
        public virtual async Task GetMeasurements_OnNoSeries_ShouldReturnEmptyCollection()
        {
            var dbName = _fixture.CreateRandomDbName();
            await _fixture.CreateEmptyDatabase(dbName);

            var result = await _fixture.Sut.Serie.GetMeasurementsAsync(dbName);
            result.Should().HaveCount(0);
        }

        [Fact]
        public virtual async Task GetMeasurements_OnExistingSeries_ShouldReturnMeasurementCollection()
        {
            var dbName = _fixture.CreateRandomDbName();
            await _fixture.CreateEmptyDatabase(dbName);
            var points = await _fixture.MockAndWritePoints(2, 2, dbName);

            var result = await _fixture.Sut.Serie.GetMeasurementsAsync(dbName);
            result.Should().HaveCount(2);
            result.FirstOrDefault(p => p.Name == points.First().Name).Name.Should().NotBeNull();
            result.FirstOrDefault(p => p.Name == points.Last().Name).Name.Should().NotBeNull();
        }

        [Fact]
        public virtual async Task CreateBatchWriter_OnBatchPointSubmission_ShouldWritePoints()
        {
            var dbName = _fixture.CreateRandomDbName();
            await _fixture.CreateEmptyDatabase(dbName);
            var batchWriter = _fixture.Sut.Serie.CreateBatchWriter(dbName);
            batchWriter.Start(1500);

            var thread1Points = _fixture.MockPoints(10);
            Task.Run(() => batchWriter.AddPoints(thread1Points)).Wait();

            var thread2Points = _fixture.MockPoints(10);
            Task.Run(() => batchWriter.AddPoints(thread2Points)).Wait();

            await Task.Delay(2000);

            var result = await _fixture.Sut.Serie.GetSeriesAsync(dbName);
            result.Should().HaveCount(2);
            result.First().Series.Should().HaveCount(10);
            result.Last().Series.Should().HaveCount(10);

            var thread3Point = _fixture.MockPoints(1).First();
            Task.Run(() => batchWriter.AddPoint(thread3Point)).Wait();

            await Task.Delay(2000);

            result = await _fixture.Sut.Serie.GetSeriesAsync(dbName);
            result.Should().HaveCount(3);
            result.Last().Series.Should().HaveCount(1);
        }

        [Fact]
        public virtual async Task CreateBatchWriter_OnBatchError_ShouldRaise()
        {
            var dbName = _fixture.CreateRandomDbName();
            await _fixture.CreateEmptyDatabase(dbName);
            var batchWriter = _fixture.Sut.Serie.CreateBatchWriter(dbName, "invalidRetention");
            batchWriter.Start();

            var errorRaised = false;
            batchWriter.OnError += (sender, e) => errorRaised = true;

            var points = _fixture.MockPoints(1);
            Task.Run(() => batchWriter.AddPoints(points)).Wait();

            await Task.Delay(1500);

            errorRaised.Should().BeTrue();
        }
    }
}
