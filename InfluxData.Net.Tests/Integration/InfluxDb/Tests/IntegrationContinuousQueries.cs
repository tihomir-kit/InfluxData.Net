using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Infrastructure;
using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb Integration")]
    [Trait("InfluxDb Integration", "Continuous Queries")]
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
            var points = await _fixture.MockAndWritePoints(1);
            var mockedCq = _fixture.MockContinuousQuery(points.First().Name);

            var result = await _fixture.Sut.ContinuousQuery.CreateContinuousQueryAsync(mockedCq);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            var cqs = await _fixture.Sut.ContinuousQuery.GetContinuousQueriesAsync(_fixture.DbName);
            var cq = cqs.FirstOrDefault(p => p.Name == mockedCq.CqName);
            cq.Should().NotBeNull();
            cq.Name.Should().Be(mockedCq.CqName);
        }

        [Fact]
        public async Task CreateContinuousQuery_OnNonExistingMeasurement_ShouldCreateContinuousQuery()
        {
            var mockedCq = _fixture.MockContinuousQuery("nonexistingseriename");

            var result = await _fixture.Sut.ContinuousQuery.CreateContinuousQueryAsync(mockedCq);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            var cqs = await _fixture.Sut.ContinuousQuery.GetContinuousQueriesAsync(_fixture.DbName);
            var cq = cqs.FirstOrDefault(p => p.Name == mockedCq.CqName);
            cq.Should().NotBeNull();
            cq.Name.Should().Be(mockedCq.CqName);
        }

        [Fact]
        public async Task CreateContinuousQuery_OnExistingCqName_ShouldThrow()
        {
            var points = await _fixture.MockAndWritePoints(1);
            var cq = await _fixture.MockAndWriteCq(points.First().Name);

            Func<Task> act = async () => { await _fixture.Sut.ContinuousQuery.CreateContinuousQueryAsync(cq); };

            act.ShouldThrow<InfluxDataApiException>();
        }

        [Fact]
        public async Task GetContinuousQueries_OnExistingCq_ShouldReturnCqs()
        {
            var points = await _fixture.MockAndWritePoints(1);
            var cq = await _fixture.MockAndWriteCq(points.First().Name);

            var cqs = await _fixture.Sut.ContinuousQuery.GetContinuousQueriesAsync(_fixture.DbName);

            var savedCq = cqs.FirstOrDefault(p => p.Name == cq.CqName);
            savedCq.Should().NotBeNull();
            savedCq.Name.Should().Be(cq.CqName);
        }

        [Fact]
        public async Task GetContinuousQueries_OnNonExistingCq_ShouldReturnEmptyCqCollection()
        {
            var dbName = _fixture.CreateRandomDbName();
            await _fixture.CreateEmptyDatabase(dbName);

            var cqs = await _fixture.Sut.ContinuousQuery.GetContinuousQueriesAsync(dbName);

            cqs.Should().NotBeNull();
            cqs.Should().HaveCount(0);
        }

        [Fact]
        public async Task DeleteContinuousQuery_OnExistingCq_ShouldReturnSuccess()
        {
            var points = await _fixture.MockAndWritePoints(1);
            var cq = await _fixture.MockAndWriteCq(points.First().Name);

            var result = await _fixture.Sut.ContinuousQuery.DeleteContinuousQueryAsync(_fixture.DbName, cq.CqName);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public void DeleteContinuousQuery_OnNonExistingCq_ShouldThrow()
        {
            Func<Task> act = async () => { await _fixture.Sut.ContinuousQuery.DeleteContinuousQueryAsync(_fixture.DbName, "nonexistingcqname"); };

            act.ShouldThrow<InfluxDataApiException>();
        }

        [Fact]
        public async Task Backfill_OnValidBackfillObject_ShouldReturnSuccess()
        {
            var backfill = _fixture.MockBackfill();

            var result = await _fixture.Sut.ContinuousQuery.BackfillAsync(_fixture.DbName, backfill);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
    }
}
