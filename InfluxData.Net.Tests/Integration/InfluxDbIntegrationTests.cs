using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Ploeh.AutoFixture;
using System.Configuration;
using System.Threading.Tasks;
using InfluxData.Net.Infrastructure.Influx;
using InfluxData.Net.Models;
using InfluxData.Net.Helpers;
using InfluxData.Net.Enums;
using Xunit;

namespace InfluxData.Net.Tests
{
    [Collection("Integration")]
    //[Trait("Integration")]
    public class InfluxDbIntegrationTests : InfluxDbIntegrationTestsBase
    { 
        [Fact]
        public async Task Influx_OnPing_ShouldReturnVersion()
        {
            var pong = await _influx.PingAsync();

            pong.Should().NotBeNull();
            pong.Success.Should().BeTrue();
            pong.Version.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Influx_OnFakeDbName_ShouldCreateAndDropDb()
        {
            // Arrange
            var dbName = CreateRandomDbName();

            // Act
            var createResponse = await _influx.CreateDatabaseAsync(dbName);
            var deleteResponse = await _influx.DropDatabaseAsync(dbName);

            // Assert
            createResponse.Success.Should().BeTrue();
            deleteResponse.Success.Should().BeTrue();
        }

        [Fact]
        public async Task DbShowDatabases_OnDatabaseExists_ShouldReturnDatabaseList()
        {
            // Arrange
            var dbName = CreateRandomDbName();
            var createResponse = await _influx.CreateDatabaseAsync(dbName);
            createResponse.Success.Should().BeTrue();

            // Act
            var databases = await _influx.ShowDatabasesAsync();

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
        public async Task DbWrite_OnMultiplePoints_ShouldWritePoints()
        {
            var points = CreateMockPoints(5);

            var writeResponse = await _influx.WriteAsync(_dbName, points);
            writeResponse.Success.Should().BeTrue();
        }

        [Fact]
        public void DbWrite_OnPointsWithoutFields_ShouldThrowException()
        {
            var points = CreateMockPoints(1);
            points.Single().Timestamp = null;
            points.Single().Fields.Clear();

            Func<Task> act = async () => { await _influx.WriteAsync(_dbName, points); };
            act.ShouldThrow<InfluxDbApiException>();
        }

        [Fact]
        public void DbQuery_OnInvalidQuery_ShouldThrowException()
        {
            Func<Task> act = async () => { await _influx.QueryAsync(_dbName, "blah"); };
            act.ShouldThrow<InfluxDbApiException>();
        }

        [Fact]
        public async Task DbQuery_OnNonExistantSeries_ShouldReturnEmptyList()
        {
            var result = await _influx.QueryAsync(_dbName, "select * from nonexistentseries");
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task DbQuery_OnNonExistantFields_ShouldReturnEmptyList()
        {
            var points = CreateMockPoints(1);
            var response = await _influx.WriteAsync(_dbName, points);

            response.Success.Should().BeTrue();

            var result = await _influx.QueryAsync(_dbName, String.Format("select nonexistentfield from \"{0}\"", points.Single().Name));
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task DbDropSeries_OnExistingSeries_ShouldDropSeries()
        {
            var points = CreateMockPoints(1);
            var writeResponse = await _influx.WriteAsync(_dbName, points);
            writeResponse.Success.Should().BeTrue();

            var expected = _influx.GetFormatter().PointToSerie(points.First());
            // query
            await Query(expected);

            var deleteSerieResponse = await _influx.DropSeriesAsync(_dbName, points.First().Name);
            deleteSerieResponse.Success.Should().BeTrue();
        }

        [Fact]
        public async Task DbQuery_OnWhereClauseNotMet_ShouldReturnNoSeries()
        {
            // Arrange
            var points = CreateMockPoints(1);
            var writeResponse = await _influx.WriteAsync(_dbName, points);
            writeResponse.Success.Should().BeTrue();

            // Act
            var queryResponse = await _influx.QueryAsync(_dbName, String.Format("select * from \"{0}\" where 0=1", points.Single().Name));

            // Assert
            queryResponse.Count.Should().Be(0);
        }

        [Fact]
        public void Formats_Point()
        {
            const string value = @"\=&,""*"" -";
            const string escapedFieldValue = @"\\=&\,\""*\""\ -";
            const string escapedTagValue = @"\\=&\,""*""\ -";
            const string seriesName = @"x";
            const string tagName = @"tag_string";
            const string fieldName = @"field_string";
            var dt = DateTime.Now;

            var point = new Point
            {
                Name = seriesName,
                Tags = new Dictionary<string, object>
                {
                    { tagName, value }
                },
                Fields = new Dictionary<string, object>
                {
                    { fieldName, value }
                },
                Timestamp = dt
            };

            var formatter = _influx.GetFormatter();
            var expected = String.Format(formatter.GetLineTemplate(),
                /* key */ seriesName + "," + tagName + "=" + escapedTagValue,
                /* fields */ fieldName + "=" + "\"" + escapedFieldValue + "\"",
                /* timestamp */ dt.ToUnixTime());

            var actual = formatter.PointToString(point);

            actual.Should().Be(expected);
        }

        [Fact]
        public void WriteRequestGetLines_OnCall_ShouldReturnNewLineSeparatedPoints()
        {
            var points = CreateMockPoints(2);
            var formatter = _influx.GetFormatter();
            var request = new WriteRequest(formatter)
            {
                Points = points
            };

            var actual = request.GetLines();
            var expected = String.Join("\n", points.Select(p => formatter.PointToString(p)));

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task CreateContinuousQuery_OnExistingMeasurement_ShouldReturnSuccess()
        {
            ContinuousQuery cq = MockContinuousQuery();

            var result = await _influx.CreateContinuousQueryAsync(cq);
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public void CreateContinuousQuery_OnNonExistingMeasurement_ShouldThrow()
        {
        }

        [Fact]
        public async Task DeleteContinuousQuery_OnExistingCq_ShouldReturnSuccess()
        {
            var cq = MockContinuousQuery();
            await _influx.CreateContinuousQueryAsync(cq);

            var result = await _influx.DeleteContinuousQueryAsync(_dbName, "FakeCQ");
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GetContinuousQueries_OnExistingCq_ShouldReturnCqs()
        {
            var cq = MockContinuousQuery();
            await _influx.CreateContinuousQueryAsync(cq);

            var result = await _influx.GetContinuousQueriesAsync(_dbName);
            result.Should().NotBeNull();
        }

        private async Task<List<Serie>> Query(Serie expected)
        {
            // 0.9.3 need 'group by' to retrieve tags as tags when using select *
            var result = await _influx.QueryAsync(_dbName, String.Format("select * from \"{0}\" group by *", expected.Name));

            result.Should().NotBeNull();
            result.Count().Should().Be(1);

            var actual = result.Single();

            actual.Name.Should().Be(expected.Name);
            actual.Tags.Count.Should().Be(expected.Tags.Count);
            actual.Tags.ShouldAllBeEquivalentTo(expected.Tags);
            actual.Columns.ShouldAllBeEquivalentTo(expected.Columns);
            actual.Columns.Count().Should().Be(expected.Columns.Count());
            actual.Values[0].Count().Should().Be(expected.Values[0].Count());
            ((DateTime)actual.Values[0][0]).ToUnixTime().Should().Be(((DateTime)expected.Values[0][0]).ToUnixTime());

            return result;
        }
    }
}
