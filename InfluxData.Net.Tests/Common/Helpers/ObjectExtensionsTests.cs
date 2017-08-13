using FluentAssertions;
using InfluxData.Net.Common.Constants;
using InfluxData.Net.Common.Helpers;
using System;
using Xunit;

namespace InfluxData.Net.Tests.Common.Helpers
{
    [Trait("Common Helpers", "Object Extensions")]
    public class ObjectExtensionsTests
    {
        private readonly DateTime _testUnixDate = new DateTime(2000, 6, 21, 5, 33, 52, 723);

        [Fact]
        public void ToUnixTime_ShouldByDefault_ReturnValidUnixTime()
        {
            var result = _testUnixDate.ToUnixTime();
            result.Should().Be(961565632723);
        }

        [Fact]
        public void ToUnixTime_ShouldWithMilliseconds_ReturnValidUnixTime()
        {
            var result = _testUnixDate.ToUnixTime(TimeUnit.Milliseconds);
            result.Should().Be(961565632723);
        }

        [Fact]
        public void ToUnixTime_ShouldWithSeconds_ReturnValidUnixTime()
        {
            var result = _testUnixDate.ToUnixTime(TimeUnit.Seconds);
            result.Should().Be(961565633);
        }

        [Fact]
        public void ToUnixTime_ShouldWithMinutes_ReturnValidUnixTime()
        {
            var result = _testUnixDate.ToUnixTime(TimeUnit.Minutes);
            result.Should().Be(16026094);
        }

        [Fact]
        public void ToUnixTime_ShouldWithHours_ReturnValidUnixTime()
        {
            var result = _testUnixDate.ToUnixTime(TimeUnit.Hours);
            result.Should().Be(267102);
        }

        [Fact]
        public void FromUnixTime_ShouldByDefault_ReturnValidDateTime()
        {
            var result = (961565632723).FromUnixTime();
            result.Date.Should().Be(_testUnixDate.Date);
            result.Hour.Should().Be(_testUnixDate.Hour);
            result.Minute.Should().Be(_testUnixDate.Minute);
            result.Second.Should().Be(_testUnixDate.Second);
        }

        [Fact]
        public void FromUnixTime_ShouldWithMilliseconds_ReturnValidDateTime()
        {
            var result = (961565632723).FromUnixTime(TimeUnit.Milliseconds);
            result.Date.Should().Be(_testUnixDate.Date);
            result.Hour.Should().Be(_testUnixDate.Hour);
            result.Minute.Should().Be(_testUnixDate.Minute);
            result.Second.Should().Be(_testUnixDate.Second);
            result.Millisecond.Should().Be(_testUnixDate.Millisecond);
        }

        [Fact]
        public void FromUnixTime_ShouldWithSeconds_ReturnValidDateTime()
        {
            var result = ((long)961565633).FromUnixTime(TimeUnit.Seconds);
            result.Date.Should().Be(_testUnixDate.Date);
            result.Hour.Should().Be(_testUnixDate.Hour);
            result.Minute.Should().Be(_testUnixDate.Minute);
        }

        [Fact]
        public void FromUnixTime_ShouldWithMinutes_ReturnValidDateTime()
        {
            var result = ((long)16026094).FromUnixTime(TimeUnit.Minutes);
            result.Date.Should().Be(_testUnixDate.Date);
            result.Hour.Should().Be(_testUnixDate.Hour);
        }

        [Fact]
        public void FromUnixTime_ShouldWithHours_ReturnValidDateTime()
        {
            var result = ((long)267102).FromUnixTime(TimeUnit.Hours);
            result.Date.Should().Be(_testUnixDate.Date);
        }
    }
}
