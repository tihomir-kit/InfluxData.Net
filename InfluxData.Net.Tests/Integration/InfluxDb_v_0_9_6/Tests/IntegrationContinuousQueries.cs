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

        public override Task CreateContinuousQuery_WithResampleStatement_ShouldCreateContinuousQuery()
        {
            // NOTE: test not applicable for this InfluxDB version
            return null;
        }
    }
}
