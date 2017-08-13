using FluentAssertions;
using InfluxData.Net.InfluxDb.ResponseParsers;
using System;
using Xunit;

namespace InfluxData.Net.Tests.InfluxDb.ResponseParsers
{
    [Trait("InfluxDb ResponseParsers", "Diagnostic")]
    public class DiagnosticsParserTests
    {
        public IDiagnosticsResponseParser Sut { get; set; }

        public DiagnosticsParserTests()
        {
            this.Sut = new DiagnosticsResponseParserProxy();
        }

        [Fact]
        public void ParseGoDuration_ShouldReturn_ValidTimeSpan()
        {
            var result = ((DiagnosticsResponseParserProxy)this.Sut).ParseGoDurationProxy("1h1m31.162222428s");

            result.Hours.Should().Be(1);
            result.Minutes.Should().Be(1);
            result.Seconds.Should().Be(31);
            result.Milliseconds.Should().Be(162);
        }

        private class DiagnosticsResponseParserProxy : DiagnosticsResponseParser
        {
            public TimeSpan ParseGoDurationProxy(string duration)
            {
                return base.ParseGoDuration(duration);
            }
        }
    }
}
