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
        public void BuildQuery_OnEverythingValid_ReturnsCorrectString()
        {
            var firstTag = "firstTag";
            var firstTagValue = "firstTagValue";

            var firstField = "firstField";
            var firstFieldValue = "firstFieldValue";

            var queryTemplate = "SELECT * FROM fakeMeasurement " +
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

            var actualNewQuery = queryTemplate.BuildQuery(param);

            Assert.Equal(expectedNewQuery, actualNewQuery);
        }


        [Fact]
        public void BuildQuery_UsingNonPrimitiveAndNonStringTypeParams_ThrowsNotSupportedException()
        {
            var firstTag = "firstTag";
            var firstTagValue = "firstTagValue";

            var firstField = "firstField";

            var queryTemplate = "SELECT * FROM fakeMeasurement " +
                       $"WHERE {firstTag} = @FirstTagValue " +
                       $"AND {firstField} = @FirstFieldValue";

            var parameters = new
            {
                @FirstTagValue = firstTagValue,
                @FirstFieldValue = new List<string>() { "NOT ACCEPTED" }
            };

            Func<string> act = () => { return queryTemplate.BuildQuery(parameters); };

            Assert.Throws(typeof(NotSupportedException), act);
        }

        // TODO: requires improvement
        //[Fact]
        //public void BuildQuery_WithMissingParameters_ThrowsArgumentException()
        //{
        //    var firstTag = "firstTag";
        //    var firstTagValue = "firstTagValue";

        //    var firstField = "firstField";

        //    var queryTemplate = "SELECT * FROM fakeMeasurement " +
        //               $"WHERE {firstTag} = @FirstTagValue " +
        //               $"AND {firstField} = @FirstFieldValue";

        //    var parameters = new
        //    {
        //        @FirstTagValue = firstTagValue
        //    };

        //    Func<string> act = () => { return queryTemplate.BuildQuery(parameters); };

        //    Assert.Throws(typeof(ArgumentException), act);
        //}
    }
}
