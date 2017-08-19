using FluentAssertions;
using InfluxData.Net.InfluxDb.Infrastructure;
using System.Linq;
using Xunit;

namespace InfluxData.Net.Tests.InfluxDb.Infrastructure
{
    [Trait("InfluxDb Infrastructure", "Request Params Builder")]
    public class RequestParamsBuilderTests
    {
        private readonly string _unencodedString = "X ; X;X [ X[X ] X]X { X{X";
        private readonly string _encodedString = "X%20%3B%20X%3BX%20%5B%20X%5BX%20%5D%20X%5DX%20%7B%20X%7BX";

        [Fact]
        public void BuildQueryRequestParams_OnUnencodedQuery_ShouldUrlEncodeIt()
        {
            var result = RequestParamsBuilder.BuildQueryRequestParams(_unencodedString);
            result.Keys.Count.Should().Be(1);
            result.First().Key.Should().Be("q");
            result.First().Value.Should().Be(_encodedString);
        }

        [Fact]
        public void BuildQueryRequestParams_OnUnencodedDbQuery_ShouldUrlEncodeIt()
        {
            var result = RequestParamsBuilder.BuildQueryRequestParams(_unencodedString, "dbName");
            result.Keys.Count.Should().Be(2);
            result["db"].Should().Be("dbName");
            result["q"].Should().Be(_encodedString);
        }

        [Fact]
        public void BuildRequestParams_OnUnencodedParams_ShouldUrlEncodeThem()
        {
            var result = RequestParamsBuilder.BuildRequestParams("dbName", "param1", _unencodedString, "param2", _unencodedString);
            result.Keys.Count.Should().Be(3);
            result["db"].Should().Be("dbName");
            result["param1"].Should().Be(_encodedString);
            result["param2"].Should().Be(_encodedString);
        }

        [Fact]
        public void BuildRequestParams_OnUnencodedParam_ShouldUrlEncodeIt()
        {
            var result = RequestParamsBuilder.BuildQueryRequestParams(_unencodedString, _unencodedString, _unencodedString);
            result.Keys.Count.Should().Be(3);
            result["q"].Should().Be(_encodedString);
            result["db"].Should().Be(_encodedString);
            result["epoch"].Should().Be(_encodedString);
        }
    }
}
