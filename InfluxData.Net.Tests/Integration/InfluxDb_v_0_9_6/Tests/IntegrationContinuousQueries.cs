using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Infrastructure;
using Xunit;

namespace InfluxData.Net.Integration.InfluxDb.Tests
{
    [Collection("InfluxDb v0.9.6 Integration")]
    [Trait("InfluxDb v0.9.6 Integration", "Continuous Queries")]
    public class IntegrationContinuousQueries_v_0_9_6 : IntegrationContinuousQueries
    {
        public IntegrationContinuousQueries_v_0_9_6(IntegrationFixture_v_0_9_6 fixture) : base(fixture)
        {
        }

        [Fact(Skip = "Test not applicable for this InfluxDB version")]
        public override Task CreateContinuousQuery_WithResampleStatement_ShouldCreateContinuousQuery()
        {
            return null;
        }

        [Fact(Skip = "Test not applicable for this InfluxDB version")]
        public override async Task CreateContinuousQuery_OnExistingCqName_NotCreateDuplicateContinuousQuery()
        {
            return;
        }

        [Fact]
        public async Task CreateContinuousQuery_OnExistingCqName_ShouldThrow()
        {
            var points = await _fixture.MockAndWritePoints(1);
            var cq = await _fixture.MockAndWriteCq(points.First().Name);

            Func<Task> act = async () => { await _fixture.Sut.ContinuousQuery.CreateContinuousQueryAsync(cq); };

            act.ShouldThrow<InfluxDataApiException>();
        }

        [Fact(Skip = "Test not applicable for this InfluxDB version")]
        public override async Task DeleteContinuousQuery_OnNonExistingCq_ShouldThrow()
        {
            return;
        }
    }
}
