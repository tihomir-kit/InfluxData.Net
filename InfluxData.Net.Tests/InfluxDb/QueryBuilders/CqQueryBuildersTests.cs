using FluentAssertions;
using InfluxData.Net.InfluxDb.QueryBuilders;
using Xunit;

namespace InfluxData.Net.Tests.InfluxDb.QueryBuilders
{
    [Trait("InfluxDb QueryBuilders", "Continuous Queries")]
    public class CqQueryBuildersTests
    {
        public ICqQueryBuilder Sut { get; set; }

        public CqQueryBuildersTests()
        {
            this.Sut = new CqQueryBuilder();
        }

        [Fact]
        public void GetCq_ShouldReturn_QueryStatement()
        {
            var query = this.Sut.GetContinuousQueries();

            query.Should().Be("SHOW CONTINUOUS QUERIES");
        }
    }
}
