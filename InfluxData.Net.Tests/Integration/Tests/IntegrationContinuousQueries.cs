using System;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using InfluxData.Net.InfluxDb.Models;

namespace InfluxData.Net.Integration.Tests
{
    [Collection("Integration")]
    [Trait("Integration", "Continuous Queries")]
    public class IntegrationContinuousQueries : IDisposable
    {
        private readonly IntegrationFixture _fixture;

        public IntegrationContinuousQueries(IntegrationFixture fixture)
        {
            _fixture = fixture;
            _fixture.TestSetup();
        }

        public void Dispose()
        {
            _fixture.TestTearDown();
        }

        [Fact]
        public async Task CreateContinuousQuery_OnExistingMeasurement_ShouldReturnSuccess()
        {
            ContinuousQuery cq = _fixture.MockContinuousQuery();

            var result = await _fixture.Sut.CreateContinuousQueryAsync(cq);
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();

            // TODO: expand test to check if the item is really in the database
        }

        [Fact]
        public void CreateContinuousQuery_OnNonExistingMeasurement_ShouldThrow()
        {
        }

        [Fact]
        public async Task DeleteContinuousQuery_OnExistingCq_ShouldReturnSuccess()
        {
            var cq = _fixture.MockContinuousQuery();
            await _fixture.Sut.CreateContinuousQueryAsync(cq);

            var result = await _fixture.Sut.DeleteContinuousQueryAsync(_fixture.DbName, "FakeCQ");
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task GetContinuousQueries_OnExistingCq_ShouldReturnCqs()
        {
            var cq = _fixture.MockContinuousQuery();
            await _fixture.Sut.CreateContinuousQueryAsync(cq);

            var result = await _fixture.Sut.GetContinuousQueriesAsync(_fixture.DbName);
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Backfill_OnValidBackfillObject_ShouldReturnSuccess()
        {
            var backfill = _fixture.MockBackfill();

            var result = await _fixture.Sut.Backfill("Novaerus01", backfill);
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
    }
}
