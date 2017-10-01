using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Helpers;
using Xunit;

namespace InfluxData.Net.Tests
{
    [Trait("InfluxDb SerieExtensions", "Serie extensions")]
    public class QueryHelpersTests
    {
        [Fact]
        public void Building_Parameterized_Query_Returns_Correct_String()
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

            var actualNewQuery = QueryHelpers.BuildParameterizedQuery(
            query,
                new
                {
                    @FirstTagValue = firstTagValue,
                    @FirstFieldValue = firstFieldValue
                });

            Assert.Equal(expectedNewQuery, actualNewQuery);
        }


        [Fact]
        public void Using_Non_Primitive_And_Non_String_Type_In_Parameters_Throws_NotSupportedException()
        {
            var firstTag = "firstTag";
            var firstTagValue = "firstTagValue";

            var firstField = "firstField";

            var query = "SELECT * FROM fakeMeasurement " +
                       $"WHERE {firstTag} = @FirstTagValue " +
                       $"AND {firstField} = @FirstFieldValue";

            Func<string> func = new Func<string>(() =>
            {
                return QueryHelpers.BuildParameterizedQuery(
                query,
                new
                {
                    @FirstTagValue = firstTagValue,
                    @FirstFieldValue = new List<string>() { "NOT ACCEPTED" }
                });
            });

            Assert.Throws(typeof(NotSupportedException), func);
        }

        [Fact]
        public void Building_Parameterized_Query_With_Missing_Parameters_Throws_ArgumentException()
        {
            var firstTag = "firstTag";
            var firstTagValue = "firstTagValue";

            var firstField = "firstField";

            var query = "SELECT * FROM fakeMeasurement " +
                       $"WHERE {firstTag} = @FirstTagValue " +
                       $"AND {firstField} = @FirstFieldValue";

            Func<string> func = new Func<string>(() =>
            {
                return QueryHelpers.BuildParameterizedQuery(
                query,
                new
                {
                    @FirstTagValue = firstTagValue
                });
            });

            Assert.Throws(typeof(ArgumentException), func);
        }
    }
}
