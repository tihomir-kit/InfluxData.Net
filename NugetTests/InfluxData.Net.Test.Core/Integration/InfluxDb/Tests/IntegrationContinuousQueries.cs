using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;
using InfluxData.Net.Common.Infrastructure;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    public abstract class IntegrationContinuousQueries : IDisposable
    {
        protected readonly IIntegrationFixture _fixture;

        public IntegrationContinuousQueries(IIntegrationFixture fixture)
        {
            _fixture = fixture;
            _fixture.TestSetup();
        }

        public void Dispose()
        {
            _fixture.TestTearDown();
        }

        [Fact]
        public virtual async Task CreateContinuousQuery_OnExistingMeasurement_ShouldCreateContinuousQuery()
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
        public virtual async Task CreateContinuousQuery_OnNonExistingMeasurement_ShouldCreateContinuousQuery()
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
        public virtual async Task CreateContinuousQuery_WithResampleStatement_ShouldCreateContinuousQuery()
        {
            var points = await _fixture.MockAndWritePoints(1);
            var mockedCq = _fixture.MockContinuousQuery(points.First().Name);
            mockedCq.Resample.For = "120m";
            mockedCq.Resample.Every = "60m";

            var result = await _fixture.Sut.ContinuousQuery.CreateContinuousQueryAsync(mockedCq);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            var cqs = await _fixture.Sut.ContinuousQuery.GetContinuousQueriesAsync(_fixture.DbName);
            var cq = cqs.FirstOrDefault(p => p.Name == mockedCq.CqName);
            cq.Should().NotBeNull();
            cq.Name.Should().Be(mockedCq.CqName);
        }

        [Fact]
        public virtual async Task CreateContinuousQuery_OnExistingCqName_NotCreateDuplicateContinuousQuery()
        {
            var points = await _fixture.MockAndWritePoints(1);
            var cq = await _fixture.MockAndWriteCq(points.First().Name);

            var result = await _fixture.Sut.ContinuousQuery.CreateContinuousQueryAsync(cq);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            var cqs = await _fixture.Sut.ContinuousQuery.GetContinuousQueriesAsync(_fixture.DbName);
            cqs.Where(p => p.Name == cq.CqName).Count().Should().Be(1);
        }

        [Fact]
        public virtual async Task GetContinuousQueries_OnExistingCq_ShouldReturnCqs()
        {
            var points = await _fixture.MockAndWritePoints(1);
            var cq = await _fixture.MockAndWriteCq(points.First().Name);

            var cqs = await _fixture.Sut.ContinuousQuery.GetContinuousQueriesAsync(_fixture.DbName);

            var savedCq = cqs.FirstOrDefault(p => p.Name == cq.CqName);
            savedCq.Should().NotBeNull();
            savedCq.Name.Should().Be(cq.CqName);
        }

        [Fact]
        public virtual async Task GetContinuousQueries_OnNonExistingCq_ShouldReturnEmptyCqCollection()
        {
            var dbName = _fixture.CreateRandomDbName();
            await _fixture.CreateEmptyDatabase(dbName);

            var cqs = await _fixture.Sut.ContinuousQuery.GetContinuousQueriesAsync(dbName);

            cqs.Should().NotBeNull();
            cqs.Should().HaveCount(0);
        }

        [Fact]
        public virtual async Task DeleteContinuousQuery_OnExistingCq_ShouldReturnSuccess()
        {
            var points = await _fixture.MockAndWritePoints(1);
            var cq = await _fixture.MockAndWriteCq(points.First().Name);

            var result = await _fixture.Sut.ContinuousQuery.DeleteContinuousQueryAsync(_fixture.DbName, cq.CqName);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }

        [Fact]
        public virtual async Task DeleteContinuousQuery_OnNonExistingCq_ShouldThrow()
        {
            Func<Task> act = async () => { await _fixture.Sut.ContinuousQuery.DeleteContinuousQueryAsync(_fixture.DbName, "nonexistingcqname"); };

            act.ShouldThrow<InfluxDataApiException>();
        }

        [Fact]
        public virtual async Task Backfill_OnValidBackfillObject_ShouldReturnSuccess()
        {
            var backfill = _fixture.MockBackfill();

            var result = await _fixture.Sut.ContinuousQuery.BackfillAsync(_fixture.DbName, backfill);

            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
        }
    }
}
