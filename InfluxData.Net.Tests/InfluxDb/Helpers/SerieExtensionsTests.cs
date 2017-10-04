using InfluxData.Net.InfluxDb.Models.Responses;
using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Helpers;
using System;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace InfluxData.Net.Tests.InfluxDb.Helpers
{
    [Trait("InfluxDb Helpers", "Serie extensions")]
    public class SerieExtensionsTests
    {
        [Fact]
        public void SeriesAs_OnRightParams_ConvertsUbiquitousSeriesToStronglyTypedCollection()
        {
            var firstSerialNumber = "F6A2B2C5E1A6";
            var secondSerialNumber = "A2B4C2F5D3D1";

            var firstCpuTemp = 54.14;
            var secondCpuTemp = 23.54;

            var firstValues = new List<IList<object>>
            {
                new List<object>
                {
                    firstSerialNumber,
                    firstCpuTemp
                }
            };

            var firstSerie = new Serie
            {
                Name = "Example serie",
                Columns = new List<string> { "serialNumber", "cpuTemp" },
                Values = firstValues
            };


            var secondValues = new List<IList<object>>
            {
                new List<object>
                {
                    secondSerialNumber,
                    secondCpuTemp
                }
            };

            var secondSerie = new Serie
            {
                Name = "Example serie",
                Columns = new List<string> { "serialNumber", "cpuTemp" },
                Values = secondValues
            };


            var series = new List<Serie>
            {
                firstSerie,
                secondSerie
            };


            var stronglyTyped = series.As<StronglyTypedSerie>();

            var expectedNumberOfConvertedValues = 2;
            var actualNumberOfConvertedValues = stronglyTyped.Count();

            var expectedSerialNumberOfFirstStronglyTypedObject = firstSerialNumber;
            var actualSerialNumberOfFirstStronglyTypedObject = stronglyTyped.First().SerialNumber;

            var expectedSerialNumberOfLastStronglyTypedObject = secondSerialNumber;
            var actualSerialNumberOfLastStronglyTypedOject = stronglyTyped.Last().SerialNumber;

            var expectedCpuTempOfFirstStronglyTypedObject = firstCpuTemp;
            var actualCpuTempOfFirstStronglyTypedObject = stronglyTyped.First().CpuTemp;

            var expectedCpuTempOfLastStronglyTypedObject = secondCpuTemp;
            var actualCpuTempOfLastStronglyTypedOject = stronglyTyped.Last().CpuTemp;

            expectedNumberOfConvertedValues.Should().Be(actualNumberOfConvertedValues);
            expectedSerialNumberOfFirstStronglyTypedObject.Should().Be(actualSerialNumberOfFirstStronglyTypedObject);
            expectedSerialNumberOfLastStronglyTypedObject.Should().Be(actualSerialNumberOfLastStronglyTypedOject);
            expectedCpuTempOfFirstStronglyTypedObject.Should().Be(actualCpuTempOfFirstStronglyTypedObject);
            expectedCpuTempOfLastStronglyTypedObject.Should().Be(actualCpuTempOfLastStronglyTypedOject);
        }

        [Fact]
        public void SeriesAs_OnInvalidFormat_ThrowsException()
        {
            var firstSerialNumber = "F6A2B2C5E1A6";

            var firstCpuTemp = "THIS CANNOT BE CONVERTED TO A DOUBLE";

            var firstValues = new List<IList<object>>
            {
                new List<object>
                {
                    firstSerialNumber,
                    firstCpuTemp
                }
            };

            var firstSerie = new Serie
            {
                Name = "Example serie",
                Columns = new List<string> { "serialNumber", "cpuTemp" },
                Values = firstValues
            };


            var series = new List<Serie>
            {
                firstSerie
            };

            Assert.Throws(typeof(FormatException), () => series.As<StronglyTypedSerie>().ToList());
        }
      
        private class StronglyTypedSerie
        {
            public string SerialNumber { get; set; }
            public double CpuTemp { get; set; }
        }
    }
}
