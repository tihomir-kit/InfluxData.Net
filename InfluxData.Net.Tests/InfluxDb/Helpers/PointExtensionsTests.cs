using InfluxData.Net.Common.Attributes;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Helpers;
using InfluxData.Net.InfluxDb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace InfluxData.Net.Tests.InfluxDb.Helpers
{
    [Trait("InfluxDb Helpers", "Point extensions")]
    public class PointExtensionsTests
    {
        [Fact]
        public void ToPoint_OnMissingMeasurementAttribute_ThrowsException()
        {   
            TestModelWithoutMeasurement model = new TestModelWithoutMeasurement
            {
                Time = DateTime.Now,
                TestTag = "TestTag",
                TestField = "TestField"
            };

            Assert.Throws<MissingExpectedAttributeException>(() => model.ToPoint());
        }

        [Fact]
        public void ToPoint_OnMultipleMeasurementAttributes_ThrowsException()
        {
            TestModelWithMultipleMeasurements model = new TestModelWithMultipleMeasurements
            {
                Time = DateTime.Now,
                Measurement1 = "FakeMeasurement1",
                Measurement2 = "FakeMeasurement2",
                TestTag = "TestTag",
                TestField = "TestField"
            };

            Assert.Throws<InvalidOperationException>(() => model.ToPoint());
        }

        [Fact]
        public void ToPoint_OnMultipleTimestampAttributes_ThrowsException()
        {
            TestModelWithMultipleTimestamps model = new TestModelWithMultipleTimestamps
            {
                Time1 = DateTime.Now,
                Time2 = DateTime.Now,
                Measurement = "FakeMeasurement",
                TestTag = "TestTag",
                TestField = "TestField"
            };

            Assert.Throws<InvalidOperationException>(() => model.ToPoint());
        }

        [Fact]
        public void ToPoint_OnNonPrimitiveTagProperties_ThrowsException()
        {
            TestModelWithNonStringTags model = new TestModelWithNonStringTags
            {
                Measurement = "FakeMeasurement",
                Time = DateTime.Now,
                PrimitiveTag = "PrimitiveTag",
                NonPrimitiveTag = new NonPrimitiveType { Whatever = "Whatever", Whatever2 = "Whatever" },
                PrimitiveField = "PrimitiveField"
            };

            Assert.Throws<InvalidOperationException>(() => model.ToPoint());
        }

        [Fact]
        public void ToPoint_OnNonPrimitiveFieldProperties_ThrowsException()
        {
            TestModelWithNonPrimitiveFields model = new TestModelWithNonPrimitiveFields
            {
                Measurement = "FakeMeasurement",
                Time = DateTime.Now,
                PrimitiveTag = "PrimitiveTag",
                PrimitiveField = "PrimitiveField",
                NonPrimitiveField = new NonPrimitiveType { Whatever = "Whatever", Whatever2 = "Whatever" }
            };

            Assert.Throws<InvalidOperationException>(() => model.ToPoint());
        }

        [Fact]
        public void ToPoint_OnValidType_IsConverted()
        {
            var measurement = "FakeMeasurement";
            var time = DateTime.Now;

            var firstTagValue = "FirstTag";
            var secondTagValue = "SecondTag";

            double firstFieldValue = 23.2;
            float secondFieldValue = 2323;

            ValidTestModel model = new ValidTestModel
            {
                Measurement = measurement,
                Time = time,
                Tag1 = firstTagValue,
                Tag2 = secondTagValue,
                PrimitiveField1 = firstFieldValue,
                PrimitiveField2 = secondFieldValue
            };

            var expectedPoint = new Point
            {
                Name = measurement,
                Timestamp = time,
                Tags = new Dictionary<string, object>
                {
                    { "Tag1", firstTagValue }, // Tag name is set explicitly in attribute name argument
                    { "tag2", secondTagValue }
                },
                Fields = new Dictionary<string, object>
                {
                    { "PrimitiveField1", firstFieldValue }, // Field name is set explicitly in attribute name argument
                    { "field2", secondFieldValue }
                }
            };

            var actualPoint = model.ToPoint();

            // Compare fields in point
            Assert.Equal(expectedPoint.Fields.Count, actualPoint.Fields.Count);
            Assert.Equal(expectedPoint.Fields.First().Key, actualPoint.Fields.First().Key);
            Assert.Equal(expectedPoint.Fields.Last().Key, actualPoint.Fields.Last().Key);
            Assert.Equal(expectedPoint.Fields.First().Value, actualPoint.Fields.First().Value);
            Assert.Equal(expectedPoint.Fields.Last().Value, actualPoint.Fields.Last().Value);

            // Compare tags in point
            Assert.Equal(expectedPoint.Tags.Count, actualPoint.Tags.Count);
            Assert.Equal(expectedPoint.Tags.First().Key, actualPoint.Tags.First().Key);
            Assert.Equal(expectedPoint.Tags.Last().Key, actualPoint.Tags.Last().Key);
            Assert.Equal(expectedPoint.Tags.First().Value, actualPoint.Tags.First().Value);
            Assert.Equal(expectedPoint.Tags.Last().Value, actualPoint.Tags.Last().Value);

            // Compare name and timestamp in point
            Assert.Equal(expectedPoint.Name, actualPoint.Name);
            Assert.Equal(expectedPoint.Timestamp, actualPoint.Timestamp);
        }

        private class TestModelWithoutMeasurement
        {
            [Timestamp]
            public DateTime Time { get; set; }

            [Tag("testtag")]
            public string TestTag { get; set; }

            [Field("testfield")]
            public string TestField { get; set; }
        }

        private class TestModelWithMultipleMeasurements
        {
            [Measurement]
            public string Measurement1 { get; set; }

            [Measurement]
            public string Measurement2 { get; set; }

            [Timestamp]
            public DateTime Time { get; set; }

            [Tag("testtag")]
            public string TestTag { get; set; }

            [Field("testfield")]
            public string TestField { get; set; }
        }

        private class TestModelWithMultipleTimestamps
        {
            [Measurement]
            public string Measurement { get; set; }

            [Timestamp]
            public DateTime Time1 { get; set; }

            [Timestamp]
            public DateTime Time2 { get; set; }

            [Tag("testtag")]
            public string TestTag { get; set; }

            [Field("testfield")]
            public string TestField { get; set; }
        }

        private class TestModelWithNonStringTags
        {
            [Measurement]
            public string Measurement { get; set; }

            [Timestamp]
            public DateTime Time { get; set; }

            [Tag("primitivetag")]
            public string PrimitiveTag { get; set; }

            [Tag("nonprimitivetag")]
            public NonPrimitiveType NonPrimitiveTag { get; set; }

            [Field("primitivetag")]
            public string PrimitiveField { get; set; }
        }

        private class TestModelWithNonPrimitiveFields
        {
            [Measurement]
            public string Measurement { get; set; }

            [Timestamp]
            public DateTime Time { get; set; }

            [Tag("primitivetag")]
            public string PrimitiveTag { get; set; }

            [Field("primitivefield")]
            public string PrimitiveField { get; set; }

            [Field("nonprimitivefield")]
            public NonPrimitiveType NonPrimitiveField { get; set; }
        }

        private class ValidTestModel
        {
            [Measurement]
            public string Measurement { get; set; }

            [Timestamp]
            public DateTime Time { get; set; }
            
            [Tag]
            public string Tag1 { get; set; }

            [Tag("tag2")]
            public string Tag2 { get; set; }

            [Field]
            public double PrimitiveField1 { get; set; }
            
            [Field("field2")]
            public float PrimitiveField2 { get; set; }
        }

        private class NonPrimitiveType
        {
            public string Whatever { get; set; }
            public string Whatever2 { get; set; }
        }
    }
}
