using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Infrastructure;

namespace InfluxData.Net.Integration.Tests
{
    [Collection("Integration")]
    [Trait("Integration", "Basic")]
    public class IntegrationBasic : IDisposable
    {
        private readonly IntegrationFixture _fixture;

        public IntegrationBasic(IntegrationFixture fixture)
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

            var formatter = _fixture.Sut.GetFormatter();
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
            var points = _fixture.CreateMockPoints(5);

            var writeResponse = await _fixture.Sut.Client.WriteAsync(_fixture.DbName, points);

            writeResponse.Success.Should().BeTrue();
            await _fixture.EnsureValidPointCount(points.First().Name, points.First().Fields.First().Key, 5);
            await _fixture.EnsurePointExists(points[2]);
        }

        [Fact]
        public void ClientWrite_OnPointsWithMissingFields_ShouldThrowException()
        {
            var points = _fixture.CreateMockPoints(1);
            points.Single().Timestamp = null;
            points.Single().Fields.Clear();

            Func<Task> act = async () => { await _fixture.Sut.Client.WriteAsync(_fixture.DbName, points); };

            act.ShouldThrow<InfluxDbApiException>();
        }

        [Fact]
        public void ClientQuery_OnInvalidQuery_ShouldThrowException()
        {
            Func<Task> act = async () => { await _fixture.Sut.Client.QueryAsync(_fixture.DbName, "blah"); };

            act.ShouldThrow<InfluxDbApiException>();
        }

        [Fact]
        public async Task ClientQuery_OnNonExistantSeries_ShouldReturnEmptySerieList()
        {
            var result = await _fixture.Sut.Client.QueryAsync(_fixture.DbName, "select * from nonexistingseries");

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ClientQuery_OnExistingPoints_ShouldReturnSerieList()
        {
            var points = _fixture.CreateMockPoints(3);
            var writeResponse = await _fixture.Sut.Client.WriteAsync(_fixture.DbName, points);
            writeResponse.Success.Should().BeTrue();
            var query = String.Format("select * from {0}", points.First().Name);

            var result = await _fixture.Sut.Client.QueryAsync(_fixture.DbName, query);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Name.Should().Be(points.First().Name);
            result.First().Values.Should().HaveCount(3);
        }

        [Fact]
        public async Task ClientQueryMultiple_OnExistingPoints_ShouldReturnSerieList()
        {
            var points = _fixture.CreateMockPoints(5).Concat(_fixture.CreateMockPoints(5)).ToArray();
            var writeResponse = await _fixture.Sut.Client.WriteAsync(_fixture.DbName, points);
            writeResponse.Success.Should().BeTrue();

            var queryies = new []
            {
                String.Format("select * from {0}", points.First().Name),
                String.Format("select * from {0}", points.Last().Name)
            };
            var result = await _fixture.Sut.Client.QueryAsync(_fixture.DbName, queryies);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().First().Name.Should().Be(points.First().Name);
            result.First().First().Values.Should().HaveCount(5);
            result.Last().First().Name.Should().Be(points.Last().Name);
            result.Last().First().Values.Should().HaveCount(5);
        }

        [Fact]
        public async Task ClientQuery_OnNonExistantFields_ShouldReturnEmptySerieList()
        {
            var points = _fixture.CreateMockPoints(1);
            var response = await _fixture.Sut.Client.WriteAsync(_fixture.DbName, points);
            response.Success.Should().BeTrue();

            var query = String.Format("select nonexistentfield from \"{0}\"", points.Single().Name);
            var result = await _fixture.Sut.Client.QueryAsync(_fixture.DbName, query);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task ClientQuery_OnWhereClauseNotMet_ShouldReturnEmptySerieList()
        {
            var points = _fixture.CreateMockPoints(1);
            var writeResponse = await _fixture.Sut.Client.WriteAsync(_fixture.DbName, points);
            writeResponse.Success.Should().BeTrue();

            var query = String.Format("select * from \"{0}\" where 0=1", points.Single().Name);
            var result = await _fixture.Sut.Client.QueryAsync(_fixture.DbName, query);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        // TODO: move to unit tests
        [Fact]
        public void WriteRequest_OnGetLines_ShouldReturnNewLineSeparatedPoints()
        {
            var points = _fixture.CreateMockPoints(2);
            var formatter = _fixture.Sut.GetFormatter();
            var request = new WriteRequest(formatter)
            {
                Points = points
            };

            var actual = request.GetLines();
            var expected = String.Join("\n", points.Select(p => formatter.PointToString(p)));

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task ClientPing_ShouldReturnVersion()
        {
            var pong = await _fixture.Sut.Client.PingAsync();

            pong.Should().NotBeNull();
            pong.Success.Should().BeTrue();
            pong.Version.Should().NotBeEmpty();
        }
    }
}
