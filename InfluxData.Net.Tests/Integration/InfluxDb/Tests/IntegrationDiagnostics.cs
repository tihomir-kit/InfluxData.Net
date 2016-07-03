using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    public abstract class IntegrationDiagnostics : IDisposable
    {
        protected readonly IIntegrationFixture _fixture;

        public IntegrationDiagnostics(IIntegrationFixture fixture)
        {
            _fixture = fixture;
            _fixture.TestSetup();
        }

        public void Dispose()
        {
            _fixture.TestTearDown();
        }

        [Fact]
        public virtual async Task Ping_ShouldReturnVersion()
        {
            var pong = await _fixture.Sut.Diagnostics.PingAsync();

            pong.Should().NotBeNull();
            pong.Success.Should().BeTrue();
            pong.Version.Should().NotBeEmpty();
        }

        [Fact]
        public virtual async Task GetStats_ShouldReturnDbStats()
        {
            var stats = await _fixture.Sut.Diagnostics.GetStatsAsync();
            stats.Runtime.Count().Should().BeGreaterThan(0);
            stats.Httpd.Count().Should().BeGreaterThan(0);
            stats.Database.Count().Should().BeGreaterThan(0);
            stats.QueryExecutor.Count().Should().BeGreaterThan(0);
            stats.Subscriber.Count().Should().BeGreaterThan(0);
        }

        [Fact]
        public virtual async Task GetDiagnostics_ShouldReturnDbDiagnostics()
        {
            var diagnostics = await _fixture.Sut.Diagnostics.GetDiagnosticsAsync();
            diagnostics.System.PID.Should().BeGreaterThan(0);
            diagnostics.Build.Version.Should().NotBeNullOrEmpty();
        }
    }
}
