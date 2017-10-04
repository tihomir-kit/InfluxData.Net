using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Helpers;
using Xunit;

namespace InfluxData.Net.Tests
{
    [Trait("InfluxDb Helpers", "Query extensions")]
    public class QueryExtensionsTests
    {
        [Fact]
        public void BuildingParameterizedQuery_OnEverythingValid_ReturnsCorrectString()
        {
            var firstTag = "firstTag";
            var firstTagValue = "firstTagValue";

            var firstField = "firstField";
            var firstFieldValue = "firstFieldValue";

            var query = "SELECT * FROM fakeMeasurement " +
                       $"WHERE {firstTag} = @FirstTagValue " +
                       $"AND {firstField} = @FirstFieldValue";

            var expectedNewQuery = "SELECT * FROM fakeMeasurement " +
                                  $"WHERE {firstTag} = {firstTagValue} " +
                                  $"AND {firstField} = {firstFieldValue}";

            var param = new
            {
                @FirstTagValue = firstTagValue,
                @FirstFieldValue = firstFieldValue
            };

            var actualNewQuery = QueryExtensions.BuildParameterizedQuery(query, param);

            Assert.Equal(expectedNewQuery, actualNewQuery);
        }


        [Fact]
        public void BuildingParameterizedQuery_UsingNonPrimitiveAndNonStringTypeParams_ThrowsNotSupportedException()
        {
            var firstTag = "firstTag";
            var firstTagValue = "firstTagValue";

            var firstField = "firstField";

            var query = "SELECT * FROM fakeMeasurement " +
                       $"WHERE {firstTag} = @FirstTagValue " +
                       $"AND {firstField} = @FirstFieldValue";

            var param = new
            {
                @FirstTagValue = firstTagValue,
                @FirstFieldValue = new List<string>() { "NOT ACCEPTED" }
            };

            Func<string> act = () => { return QueryExtensions.BuildParameterizedQuery(query, param); };

            // act.ShouldThrow<NotSupportedException>();

            Assert.Throws(typeof(NotSupportedException), act);
        }

        [Fact]
        public void BuildingParameterizedQuery_WithMissingParameters_ThrowsArgumentException()
        {
            var firstTag = "firstTag";
            var firstTagValue = "firstTagValue";

            var firstField = "firstField";

            var query = "SELECT * FROM fakeMeasurement " +
                       $"WHERE {firstTag} = @FirstTagValue " +
                       $"AND {firstField} = @FirstFieldValue";

            var param = new
            {
                @FirstTagValue = firstTagValue
            };

            Func<string> act = () => { return QueryExtensions.BuildParameterizedQuery(query, param); };

            Assert.Throws(typeof(ArgumentException), act);
        }
    }
}
