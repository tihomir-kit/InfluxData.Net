using System;
using FluentAssertions;
using System.Threading.Tasks;
using Xunit;
using InfluxData.Net.InfluxDb.Models;
using System.Linq;
using InfluxData.Net.InfluxDb.Infrastructure;

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
        public async Task CreateContinuousQuery_OnExistingMeasurement_ShouldCreateContinuousQuery()
        {
            var points = _fixture.CreateMockPoints(1);
            var writeResponse = await _fixture.Sut.Client.WriteAsync(_fixture.DbName, points);
            writeResponse.Success.Should().BeTrue();
            var cq = _fixture.MockContinuousQuery(points.First().Name);

            var result = await _fixture.Sut.ContinuousQuery.CreateContinuousQueryAsync(cq);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            var cqs = await _fixture.Sut.ContinuousQuery.GetContinuousQueriesAsync(_fixture.DbName);
            var savedCq = cqs.FirstOrDefault(p => p.Name == cq.CqName);
            savedCq.Should().NotBeNull();
            savedCq.Name.Should().Be(cq.CqName);
        }

        [Fact]
        public async Task CreateContinuousQuery_OnNonExistingMeasurement_ShouldCreateContinuousQuery()
        {
            var cq = _fixture.MockContinuousQuery("nonexistingseriename");

            var result = await _fixture.Sut.ContinuousQuery.CreateContinuousQueryAsync(cq);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            var cqs = await _fixture.Sut.ContinuousQuery.GetContinuousQueriesAsync(_fixture.DbName);
            var savedCq = cqs.FirstOrDefault(p => p.Name == cq.CqName);
            savedCq.Should().NotBeNull();
            savedCq.Name.Should().Be(cq.CqName);
        }

        [Fact]
        public async Task CreateContinuousQuery_OnExistingCqName_ShouldThrow()
        {
            var points = _fixture.CreateMockPoints(1);
            var writeResponse = await _fixture.Sut.Client.WriteAsync(_fixture.DbName, points);
            writeResponse.Success.Should().BeTrue();
            var cq = _fixture.MockContinuousQuery(points.First().Name);
            await _fixture.Sut.ContinuousQuery.CreateContinuousQueryAsync(cq);

            Func<Task> act = async () => { await _fixture.Sut.ContinuousQuery.CreateContinuousQueryAsync(cq); };

            act.ShouldThrow<InfluxDbApiException>();
        }

        [Fact]
        public async Task DeleteContinuousQuery_OnExistingCq_ShouldReturnSuccess()
        {
            var points = _fixture.CreateMockPoints(1);
            var writeResponse = await _fixture.Sut.Client.WriteAsync(_fixture.DbName, points);
            writeResponse.Success.Should().BeTrue();
            var cq = _fixture.MockContinuousQuery(points.First().Name);
            await _fixture.Sut.ContinuousQuery.CreateContinuousQueryAsync(cq);

            var result = await _fixture.Sut.ContinuousQuery.DeleteContinuousQueryAsync(_fixture.DbName, cq.CqName);
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public void DeleteContinuousQuery_OnNonExistingCq_ShouldThrow()
        {
            Func<Task> act = async () => { await _fixture.Sut.ContinuousQuery.DeleteContinuousQueryAsync(_fixture.DbName, "nonexistingcqname"); };

            act.ShouldThrow<InfluxDbApiException>();
        }

        [Fact]
        public async Task GetContinuousQueries_OnExistingCq_ShouldReturnCqs()
        {
            var points = _fixture.CreateMockPoints(1);
            var writeResponse = await _fixture.Sut.Client.WriteAsync(_fixture.DbName, points);
            writeResponse.Success.Should().BeTrue();
            var cq = _fixture.MockContinuousQuery(points.First().Name);
            var result = await _fixture.Sut.ContinuousQuery.CreateContinuousQueryAsync(cq);
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();

            var cqs = await _fixture.Sut.ContinuousQuery.GetContinuousQueriesAsync(_fixture.DbName);
            var savedCq = cqs.FirstOrDefault(p => p.Name == cq.CqName);
            savedCq.Should().NotBeNull();
            savedCq.Name.Should().Be(cq.CqName);
        }

        [Fact]
        public async Task GetContinuousQueries_OnNonExistingCq_ShouldReturnEmptyCqList()
        {
            //var cqs = await _fixture.Sut.ContinuousQuery.GetContinuousQueriesAsync(_fixture.DbName);
            //cqs.Should().NotBeNull();
            //cqs.Should().HaveCount(0);
        }

        [Fact]
        public async Task Backfill_OnValidBackfillObject_ShouldReturnSuccess()
        {
            var backfill = _fixture.MockBackfill();

            var result = await _fixture.Sut.ContinuousQuery.BackfillAsync("Novaerus01", backfill);
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
    }
}
