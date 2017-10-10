using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using Xunit;
using InfluxData.Net.Common.Constants;

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
        public virtual void Formatter_ShouldFormatPointProperly()
        {
            const string value = @"\=&,""*"" -";
            const string seriesName = @"x";
            const string tagName = @"tag_string";
            const string escapedTagValue = @"\\=&\,""*""\ -";
            const string fieldName = @"field_string";
            const string escapedFieldValue = @"\=&,""*"" -";
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

            var formatter = _fixture.Sut.RequestClient.GetPointFormatter();

            var expected = $"{seriesName},{tagName}={escapedTagValue} " + // key
                           $"{fieldName}=\"{escapedFieldValue}\" " +      // fields
                           $"{dt.ToUnixTime()}";                          // timestamp

            var actual = formatter.PointToString(point);

            actual.Should().Be(expected);
        }

        [Fact]
        public virtual async Task ClientWrite_OnValidPointsToSave_ShouldWriteSuccessfully()
        {
            var points = _fixture.MockPoints(5);

            var writeResponse = await _fixture.Sut.Client.WriteAsync(points, _fixture.DbName);

            writeResponse.Success.Should().BeTrue();
            await _fixture.EnsureValidPointCount(points.First().Name, points.First().Fields.First().Key, 5);
            await _fixture.EnsurePointExists(points.ToArray()[2]);
        }

        [Fact]
        public virtual async Task ClientWrite_OnTimeUnitHours_ShouldWriteSuccessfully()
        {
            var point = _fixture.MockPoints(1).Single();

            var writeResponse = await _fixture.Sut.Client.WriteAsync(point, _fixture.DbName, precision: TimeUnit.Hours);
            writeResponse.Success.Should().BeTrue();
            await _fixture.EnsureValidPointCount(point.Name, point.Fields.First().Key, 1);
            var serie = await _fixture.EnsurePointExists(point, TimeUnit.Hours);

            var pointTimestamp = (DateTime)point.Timestamp;
            var serieTimestamp = (DateTime)serie.Values[0][0];
            serieTimestamp.Date.Should().Be(pointTimestamp.Date);
        }

        [Fact]
        public virtual async Task ClientWrite_OnTimeUnitMinutes_ShouldWriteSuccessfully()
        {
            var point = _fixture.MockPoints(1).Single();

            var writeResponse = await _fixture.Sut.Client.WriteAsync(point, _fixture.DbName, precision: TimeUnit.Minutes);
            writeResponse.Success.Should().BeTrue();
            await _fixture.EnsureValidPointCount(point.Name, point.Fields.First().Key, 1);
            var serie = await _fixture.EnsurePointExists(point, TimeUnit.Minutes);

            var pointTimestamp = (DateTime)point.Timestamp;
            var serieTimestamp = (DateTime)serie.Values[0][0];
            serieTimestamp.Date.Should().Be(pointTimestamp.Date);
            serieTimestamp.Hour.Should().Be(pointTimestamp.Hour);
        }

        /// <see cref="https://github.com/pootzko/InfluxData.Net/issues/25"/>
        [Fact]
        public virtual async Task ClientWrite_OnTimeUnitSeconds_ShouldWriteSuccessfully()
        {
            var point = _fixture.MockPoints(1).Single();

            var writeResponse = await _fixture.Sut.Client.WriteAsync(point, _fixture.DbName, precision: TimeUnit.Seconds);
            writeResponse.Success.Should().BeTrue();
            await _fixture.EnsureValidPointCount(point.Name, point.Fields.First().Key, 1);
            var serie = await _fixture.EnsurePointExists(point, TimeUnit.Seconds);

            var pointTimestamp = (DateTime)point.Timestamp;
            var serieTimestamp = (DateTime)serie.Values[0][0];
            serieTimestamp.Date.Should().Be(pointTimestamp.Date);
            serieTimestamp.Hour.Should().Be(pointTimestamp.Hour);
            serieTimestamp.Minute.Should().Be(pointTimestamp.Minute);
        }


        [Fact]
        public virtual async Task ClientWrite_OnTimeUnitMillis_ShouldWriteSuccessfully()
        {
            var point = _fixture.MockPoints(1).Single();

            var writeResponse = await _fixture.Sut.Client.WriteAsync(point, _fixture.DbName, precision: TimeUnit.Milliseconds);
            writeResponse.Success.Should().BeTrue();
            await _fixture.EnsureValidPointCount(point.Name, point.Fields.First().Key, 1);
            var serie = await _fixture.EnsurePointExists(point, TimeUnit.Milliseconds);

            var pointTimestamp = (DateTime)point.Timestamp;
            var serieTimestamp = (DateTime)serie.Values[0][0];
            serieTimestamp.Date.Should().Be(pointTimestamp.Date);
            serieTimestamp.Hour.Should().Be(pointTimestamp.Hour);
            serieTimestamp.Minute.Should().Be(pointTimestamp.Minute);
            serieTimestamp.Second.Should().Be(pointTimestamp.Second);
        }

        [Fact]
        public virtual async Task ClientWrite_OnTimeUnitDefault_ShouldWriteSuccessfully()
        {
            var points = await _fixture.MockAndWritePoints(1);
            var point = points.Single();

            await _fixture.EnsureValidPointCount(point.Name, point.Fields.First().Key, 1);
            var serie = await _fixture.EnsurePointExists(point);

            var pointTimestamp = (DateTime)point.Timestamp;
            var serieTimestamp = (DateTime)serie.Values[0][0];
            serieTimestamp.Date.Should().Be(pointTimestamp.Date);
            serieTimestamp.Hour.Should().Be(pointTimestamp.Hour);
            serieTimestamp.Minute.Should().Be(pointTimestamp.Minute);
            serieTimestamp.Second.Should().Be(pointTimestamp.Second);
        }

        [Fact]
        public virtual void ClientWrite_OnPointsWithMissingFields_ShouldThrowException()
        {
            var points = _fixture.MockPoints(1);
            points.Single().Timestamp = null;
            points.Single().Fields.Clear();

            Func<Task> act = async () => { await _fixture.Sut.Client.WriteAsync(points, _fixture.DbName); };

            act.ShouldThrow<InfluxDataApiException>();
        }

        [Fact]
        public virtual void ClientQuery_OnInvalidQuery_ShouldThrowException()
        {
            Func<Task> act = async () => { await _fixture.Sut.Client.QueryAsync(_fixture.DbName, "blah"); };

            act.ShouldThrow<InfluxDataApiException>();
        }

        [Fact]
        public virtual async Task ClientQuery_OnNonExistantSeries_ShouldReturnEmptySerieCollection()
        {
            var result = await _fixture.Sut.Client.QueryAsync("select * from nonexistingseries", _fixture.DbName);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public virtual async Task ClientQuery_OnExistingPoints_ShouldReturnSerieCollection()
        {
            var points = await _fixture.MockAndWritePoints(3);

            var query = String.Format("select * from {0}", points.First().Name);
            var result = await _fixture.Sut.Client.QueryAsync(query, _fixture.DbName);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Name.Should().Be(points.First().Name);
            result.First().Values.Should().HaveCount(3);
        }

        [Fact]
        public virtual async Task ClientQuery_Parameterized_OnExistingPoints_ShouldReturnSerieCollection()
        {
            var points = await _fixture.MockAndWritePoints(3);

            var firstTag = points.First().Tags.First().Key;
            var firstTagValue = points.First().Tags.First().Value;

            var firstField = points.First().Fields.First().Key;
            var firstFieldValue = points.First().Fields.First().Value;

            var query = $"SELECT * FROM {points.First().Name} " +
                        $@"WHERE {firstTag} = '@FirstTagValueParam' " +
                        $@"AND {firstField} = @FirstFieldValueParam";


            var result = await _fixture.Sut.Client.QueryAsync(
                query,
                new
                {
                    @FirstTagValueParam = firstTagValue,
                    @FirstFieldValueParam = firstFieldValue
                }, _fixture.DbName);

            var t = result.First();

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Name.Should().Be(points.First().Name);
            result.First().Values.Should().HaveCount(1);
        }

        [Fact]
        public virtual async Task ClientQueryMultiple_OnExistingPoints_ShouldReturnSerieCollection()
        {
            var points = await _fixture.MockAndWritePoints(5, 2);

            var pointNames = points.Select(p => p.Name).Distinct();
            pointNames.Should().HaveCount(2);

            var queries = new []
            {
                String.Format("select * from {0}", pointNames.First()),
                String.Format("select * from {0}", pointNames.Last())
            };
            var result = await _fixture.Sut.Client.QueryAsync(queries, _fixture.DbName);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Name.Should().Be(points.First().Name);
            result.First().Values.Should().HaveCount(5);
            result.Last().Name.Should().Be(points.Last().Name);
            result.Last().Values.Should().HaveCount(5);
        }

        [Fact]
        public virtual async Task ClientQueryMultiple_WithOneExistantSeriesQuery_ShouldReturnSingleSerie()
        {
            var points = await _fixture.MockAndWritePoints(6);

            var queries = new[]
            {
                String.Format("select * from {0}", "nonexistingseries"),
                String.Format("select * from {0}", points.First().Name)
            };
            var result = await _fixture.Sut.Client.QueryAsync(queries, _fixture.DbName);

            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Name.Should().Be(points.First().Name);
            result.First().Values.Should().HaveCount(6);
        }

        [Fact]
        public virtual async Task ClientMultiQuery_OnExistingPoints_ShouldReturnSerieResultCollection()
        {
            var points = await _fixture.MockAndWritePoints(4, 2);

            var pointNames = points.Select(p => p.Name).Distinct();
            pointNames.Should().HaveCount(2);

            var queries = new []
            {
                String.Format("select * from {0}", pointNames.First()),
                String.Format("select * from {0}", pointNames.Last())
            };
            var result = await _fixture.Sut.Client.MultiQueryAsync(queries, _fixture.DbName);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().First().Name.Should().Be(points.First().Name);
            result.First().First().Values.Should().HaveCount(4);
            result.Last().First().Name.Should().Be(points.Last().Name);
            result.Last().First().Values.Should().HaveCount(4);
        }

        [Fact]
        public virtual async Task ClientMultiQuery_WithOneExistantSeriesQuery_ShouldReturnEmptyAndPopulatedSeries()
        {
            var points = await _fixture.MockAndWritePoints(4);

            var queries = new[]
            {
                String.Format("select * from {0}", "nonexistingseries"),
                String.Format("select * from {0}", points.First().Name)
            };
            var result = await _fixture.Sut.Client.MultiQueryAsync(queries, _fixture.DbName);

            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.First().Should().HaveCount(0);
            result.Last().First().Name.Should().Be(points.First().Name);
            result.Last().First().Values.Should().HaveCount(4);
        }

        [Fact]
        public virtual async Task ClientQuery_OnNonExistantFields_ShouldReturnEmptySerieCollection()
        {
            var points = await _fixture.MockAndWritePoints(1);

            var query = String.Format("select nonexistentfield from \"{0}\"", points.Single().Name);
            var result = await _fixture.Sut.Client.QueryAsync(query, _fixture.DbName);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public virtual async Task ClientQuery_OnWhereClauseNotMet_ShouldReturnEmptySerieCollection()
        {
            var points = await _fixture.MockAndWritePoints(1);

            var query = String.Format("select * from \"{0}\" where 0=1", points.Single().Name);
            var result = await _fixture.Sut.Client.QueryAsync(query, _fixture.DbName);

            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        // TODO: move to unit tests
        [Fact]
        public virtual void WriteRequest_OnGetLines_ShouldReturnNewLineSeparatedPoints()
        {
            var points = _fixture.MockPoints(2);
            var formatter = _fixture.Sut.RequestClient.GetPointFormatter();
            var request = new WriteRequest(formatter)
            {
                Points = points
            };

            var actual = request.GetLines();
            var expected = String.Join("\n", points.Select(p => formatter.PointToString(p)));

            actual.Should().Be(expected);
        }

        [Fact]
        public virtual async Task ClientWrite_OnWhitespaceInFieldValue_ShouldNotSaveEscapedWhitespace()
        {
            var fieldName = "field_with_whitespace";
            var fieldValue = "some value with whitespace";

            var point = new Point
            {
                Name = "whitespaceEscapeTest",
                Fields = new Dictionary<string, object>
                {
                    { fieldName, fieldValue },
                },
                Timestamp = DateTime.UtcNow
            };
            var writeResponse = await _fixture.Sut.Client.WriteAsync(point, _fixture.DbName);

            writeResponse.Success.Should().BeTrue();

            await _fixture.EnsureValidPointCount(point.Name, point.Fields.First().Key, 1);
            var query = String.Format("select * from {0}", point.Name);
            var result = await _fixture.Sut.Client.QueryAsync(query, _fixture.DbName);

            result.Count().Should().Be(1);
            var serie = result.First();
            var fwwIndex = serie.Columns.IndexOf(fieldName);
            var fwwValue = serie.Values.First()[fwwIndex];

            fwwValue.Should().Be(fieldValue);
        }
    }
}
