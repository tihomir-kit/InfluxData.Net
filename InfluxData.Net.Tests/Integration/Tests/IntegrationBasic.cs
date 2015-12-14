using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using System.Threading.Tasks;
using InfluxData.Net.Infrastructure.Influx;
using InfluxData.Net.Models;
using InfluxData.Net.Helpers;
using Xunit;

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

            var formatter = _fixture.Sut.GetFormatter();
            var expected = String.Format(formatter.GetLineTemplate(),
                /* key */ seriesName + "," + tagName + "=" + escapedTagValue,
                /* fields */ fieldName + "=" + "\"" + escapedFieldValue + "\"",
                /* timestamp */ dt.ToUnixTime());

            var actual = formatter.PointToString(point);

            actual.Should().Be(expected);
        }

        [Fact]
        public async Task Influx_OnPing_ShouldReturnVersion()
        {
            var pong = await _fixture.Sut.PingAsync();

            pong.Should().NotBeNull();
            pong.Success.Should().BeTrue();
            pong.Version.Should().NotBeEmpty();
        }


        [Fact]
        public async Task DbWrite_OnMultiplePoints_ShouldWritePoints()
        {
            var points = _fixture.CreateMockPoints(5);

            var writeResponse = await _fixture.Sut.WriteAsync(_fixture.DbName, points);
            writeResponse.Success.Should().BeTrue();
        }

        [Fact]
        public void DbWrite_OnPointsWithoutFields_ShouldThrowException()
        {
            var points = _fixture.CreateMockPoints(1);
            points.Single().Timestamp = null;
            points.Single().Fields.Clear();

            Func<Task> act = async () => { await _fixture.Sut.WriteAsync(_fixture.DbName, points); };
            act.ShouldThrow<InfluxDbApiException>();
        }

        [Fact]
        public void DbQuery_OnInvalidQuery_ShouldThrowException()
        {
            Func<Task> act = async () => { await _fixture.Sut.QueryAsync(_fixture.DbName, "blah"); };
            act.ShouldThrow<InfluxDbApiException>();
        }

        [Fact]
        public async Task DbQuery_OnNonExistantSeries_ShouldReturnEmptyList()
        {
            var result = await _fixture.Sut.QueryAsync(_fixture.DbName, "select * from nonexistentseries");
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task DbQuery_OnNonExistantFields_ShouldReturnEmptyList()
        {
            var points = _fixture.CreateMockPoints(1);
            var response = await _fixture.Sut.WriteAsync(_fixture.DbName, points);

            response.Success.Should().BeTrue();

            var result = await _fixture.Sut.QueryAsync(_fixture.DbName, String.Format("select nonexistentfield from \"{0}\"", points.Single().Name));
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task DbQuery_OnWhereClauseNotMet_ShouldReturnNoSeries()
        {
            // Arrange
            var points = _fixture.CreateMockPoints(1);
            var writeResponse = await _fixture.Sut.WriteAsync(_fixture.DbName, points);
            writeResponse.Success.Should().BeTrue();

            // Act
            var queryResponse = await _fixture.Sut.QueryAsync(_fixture.DbName, String.Format("select * from \"{0}\" where 0=1", points.Single().Name));

            // Assert
            queryResponse.Count.Should().Be(0);
        }

        [Fact]
        public void WriteRequestGetLines_OnCall_ShouldReturnNewLineSeparatedPoints()
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
    }
}
