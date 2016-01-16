using FluentAssertions;
using InfluxData.Net.InfluxDb.QueryBuilders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace InfluxData.Net.Tests.QueryBuilders
{
    [Trait("QueryBuilders", "Continuous Queries")]
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
