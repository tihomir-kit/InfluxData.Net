using FluentAssertions;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v1.0.0 Integration")]
    [Trait("InfluxDb v1.0.0 Integration", "Basic")]
    public class IntegrationBasic_v_1_0_0 : IntegrationBasic
    {
        public IntegrationBasic_v_1_0_0(IntegrationFixture_v_1_0_0 fixture) : base(fixture)
        {
        }

        [Fact]
        public override void Formatter_ShouldFormatPointProperly()
        {
            const string value = @"\=&,""*"" -";
            const string seriesName = @"x";
            const string tagName = @"tag_string";
            const string escapedTagValue = @"\\=&\,""*""\ -";
            const string fieldName = @"field_string";
            const string escapedFieldValue = @"\\\=&\,\""*\""\ -";
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

        [Fact(Skip = "Test not applicable for this InfluxDB version")]
        public override Task ClientWrite_OnWhitespaceInFieldValue_ShouldNotSaveEscapedWhitespace()
        {
            return null;
        }

        /// <see cref="https://github.com/pootzko/InfluxData.Net/issues/26"/>
        [Fact]
        public virtual async Task ClientWrite_OnBackslashInPointField_ShouldWriteSuccessfully()
        {
            var point = new Point
            {
                Name = "test",
                Fields = new Dictionary<string, object>
                {
                    { "test", @"backslash\" },
                },
                Timestamp = DateTime.UtcNow
            };

            var writeResponse = await _fixture.Sut.Client.WriteAsync(point, _fixture.DbName);

            writeResponse.Success.Should().BeTrue();
            await Task.Delay(1000); // Without this, the test often fails because Influx doesn't flush the new point fast enough
            await _fixture.EnsureValidPointCount(point.Name, point.Fields.First().Key, 1);
            var serie = await _fixture.EnsurePointExists(point);
            serie.Values[0][1].Should().Be(point.Fields.First().Value);
        }
    }
}
