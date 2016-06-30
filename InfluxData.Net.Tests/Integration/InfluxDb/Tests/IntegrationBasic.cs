using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    public abstract class IntegrationBasic : IDisposable
    {
        protected readonly IIntegrationFixture _fixture;

        public IntegrationBasic(IIntegrationFixture fixture)
        {
            _fixture = fixture;
            _fixture.TestSetup();
        }

        public void Dispose()
        {
            _fixture.TestTearDown();
        }

        [Fact]
        public void Formatter_OnGetLineTemplate_ShouldFormatPoint()
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

            var formatter = _fixture.Sut.GetPointFormatter();
            var expected = String.Format(formatter.GetLineTemplate(),
                /* key */ seriesName + "," + tagName + "=" + escapedTagValue,
                /* fields */ fieldName + "=" + "\"" + escapedFieldValue + "\"",
                /* timestamp */ dt.ToUnixTime());

            var actual = formatter.PointToString(point);

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task ClientWrite_OnValidPointsToSave_ShouldWriteSuccessfully()
        {
            var points = _fixture.MockPoints(5);

            var writeResponse = await _fixture.Sut.Client.WriteAsync(_fixture.DbName, points);

            writeResponse.Success.Should().BeTrue();
            await _fixture.EnsureValidPointCount(points.First().Name, points.First().Fields.First().Key, 5);
            await _fixture.EnsurePointExists(points.ToArray()[2]);
        }

        [Fact]
        public void ClientWrite_OnPointsWithMissingFields_ShouldThrowException()
        {
            var points = _fixture.MockPoints(1);
            points.Single().Timestamp = null;
            points.Single().Fields.Clear();

            Func<Task> act = async () => { await _fixture.Sut.Client.WriteAsync(_fixture.DbName, points); };

            act.ShouldThrow<InfluxDataApiException>();
        }

        [Fact]
        public void ClientQuery_OnInvalidQuery_ShouldThrowException()
        {
            Func<Task> act = async () => { await _fixture.Sut.Client.QueryAsync(_fixture.DbName, "blah"); };

            act.ShouldThrow<InfluxDataApiException>();
        }

        [Fact]
        public async Task ClientQuery_OnNonExistantSeries_ShouldReturnEmptySerieCollection()
        {
            var result = await _fixture.Sut.Client.QueryAsync(_fixture.DbName, "select * from nonexistingseries");

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ClientQuery_OnExistingPoints_ShouldReturnSerieCollection()
        {
            var points = await _fixture.MockAndWritePoints(3);

            var query = String.Format("select * from {0}", points.First().Name);
            var result = await _fixture.Sut.Client.QueryAsync(_fixture.DbName, query);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Name.Should().Be(points.First().Name);
            result.First().Values.Should().HaveCount(3);
        }

        [Fact]
        public async Task ClientQueryMultiple_OnExistingPoints_ShouldReturnSerieCollection()
        {
            var points = await _fixture.MockAndWritePoints(5, 2);

            var pointNames = points.Select(p => p.Name).Distinct();
            pointNames.Should().HaveCount(2);

            var queries = new []
            {
                String.Format("select * from {0}", pointNames.First()),
                String.Format("select * from {0}", pointNames.Last())
            };
            var result = await _fixture.Sut.Client.QueryAsync(_fixture.DbName, queries);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Name.Should().Be(points.First().Name);
            result.First().Values.Should().HaveCount(5);
            result.Last().Name.Should().Be(points.Last().Name);
            result.Last().Values.Should().HaveCount(5);
        }

        [Fact]
        public async Task ClientQueryMultiple_WithOneExistantSeriesQuery_ShouldReturnSingleSerie()
        {
            var points = await _fixture.MockAndWritePoints(6);

            var queries = new[]
            {
                String.Format("select * from {0}", "nonexistingseries"),
                String.Format("select * from {0}", points.First().Name)
            };
            var result = await _fixture.Sut.Client.QueryAsync(_fixture.DbName, queries);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Name.Should().Be(points.First().Name);
            result.First().Values.Should().HaveCount(6);
        }

        [Fact]
        public async Task ClientMultiQuery_OnExistingPoints_ShouldReturnSerieResultCollection()
        {
            var points = await _fixture.MockAndWritePoints(4, 2);

            var pointNames = points.Select(p => p.Name).Distinct();
            pointNames.Should().HaveCount(2);

            var queries = new []
            {
                String.Format("select * from {0}", pointNames.First()),
                String.Format("select * from {0}", pointNames.Last())
            };
            var result = await _fixture.Sut.Client.MultiQueryAsync(_fixture.DbName, queries);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().First().Name.Should().Be(points.First().Name);
            result.First().First().Values.Should().HaveCount(4);
            result.Last().First().Name.Should().Be(points.Last().Name);
            result.Last().First().Values.Should().HaveCount(4);
        }

        [Fact]
        public async Task ClientMultiQuery_WithOneExistantSeriesQuery_ShouldReturnEmptyAndPopulatedSeries()
        {
            var points = await _fixture.MockAndWritePoints(4);

            var queries = new[]
            {
                String.Format("select * from {0}", "nonexistingseries"),
                String.Format("select * from {0}", points.First().Name)
            };
            var result = await _fixture.Sut.Client.MultiQueryAsync(_fixture.DbName, queries);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Should().HaveCount(0);
            result.Last().First().Name.Should().Be(points.First().Name);
            result.Last().First().Values.Should().HaveCount(4);
        }

        [Fact]
        public async Task ClientQuery_OnNonExistantFields_ShouldReturnEmptySerieCollection()
        {
            var points = await _fixture.MockAndWritePoints(1);

            var query = String.Format("select nonexistentfield from \"{0}\"", points.Single().Name);
            var result = await _fixture.Sut.Client.QueryAsync(_fixture.DbName, query);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ClientQuery_OnWhereClauseNotMet_ShouldReturnEmptySerieCollection()
        {
            var points = await _fixture.MockAndWritePoints(1);

            var query = String.Format("select * from \"{0}\" where 0=1", points.Single().Name);
            var result = await _fixture.Sut.Client.QueryAsync(_fixture.DbName, query);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        // TODO: move to unit tests
        [Fact]
        public void WriteRequest_OnGetLines_ShouldReturnNewLineSeparatedPoints()
        {
            var points = _fixture.MockPoints(2);
            var formatter = _fixture.Sut.GetPointFormatter();
            var request = new WriteRequest(formatter)
            {
                Points = points
            };

            var actual = request.GetLines();
            var expected = String.Join("\n", points.Select(p => formatter.PointToString(p)));

            actual.Should().Be(expected);
        }
    }
}
