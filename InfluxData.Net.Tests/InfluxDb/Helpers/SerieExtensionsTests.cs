using InfluxData.Net.InfluxDb.Models.Responses;
using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Helpers;
using System;
using System.Linq;
using Xunit;

namespace InfluxData.Net.Tests.InfluxDb.Helpers
{
    [Trait("InfluxDb SerieExtensions", "Serie extensions")]
    public class SerieExtensionsTests
    {

        [Fact]
        public void Can_Convert_Ubiquitous_Series_To_Strongly_Typed_Collection()
        {
            var firstSerialNumber = "F6A2B2C5E1A6";
            var secondSerialNumber = "A2B4C2F5D3D1";

            var firstCpuTemp = 54.14;
            var secondCpuTemp = 23.54;

            IList<IList<object>> firstValues = new List<IList<object>>
                {
                    new List<object>
                    {
                        firstSerialNumber,
                        firstCpuTemp
                    }
                };

            Serie firstSerie = new Serie
            {
                Name = "Example serie",
                Columns = new List<string> { "serialNumber", "cpuTemp" },
                Values = firstValues
            };


            IList<IList<object>> secondValues = new List<IList<object>>
                {
                    new List<object>
                    {
                        secondSerialNumber,
                        secondCpuTemp
                    }
                };

            Serie secondSerie = new Serie
            {
                Name = "Example serie",
                Columns = new List<string> { "serialNumber", "cpuTemp" },
                Values = secondValues
            };


            List<Serie> series = new List<Serie>
            {
                firstSerie,
                secondSerie
            };


            var stronglyTyped = series.RecordsAs<StronglyTypedSerie>();

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

            Assert.Equal(expectedNumberOfConvertedValues, actualNumberOfConvertedValues);
            Assert.Equal(expectedSerialNumberOfFirstStronglyTypedObject, actualSerialNumberOfFirstStronglyTypedObject);
            Assert.Equal(expectedSerialNumberOfLastStronglyTypedObject, actualSerialNumberOfLastStronglyTypedOject);
            Assert.Equal(expectedCpuTempOfFirstStronglyTypedObject, actualCpuTempOfFirstStronglyTypedObject);
            Assert.Equal(expectedCpuTempOfLastStronglyTypedObject, actualCpuTempOfLastStronglyTypedOject);
        }

        [Fact]
        public void Trying_To_Convert_With_Invalid_Format_Throws_Exception()
        {
            var firstSerialNumber = "F6A2B2C5E1A6";

            var firstCpuTemp = "THIS CANNOT BE CONVERTED TO A DOUBLE";

            IList<IList<object>> firstValues = new List<IList<object>>
                {
                    new List<object>
                    {
                        firstSerialNumber,
                        firstCpuTemp
                    }
                };

            Serie firstSerie = new Serie
            {
                Name = "Example serie",
                Columns = new List<string> { "serialNumber", "cpuTemp" },
                Values = firstValues
            };


            List<Serie> series = new List<Serie>
            {
                firstSerie
            };

            Assert.Throws(typeof(FormatException), () => series.RecordsAs<StronglyTypedSerie>().ToList());
        }
      
        class StronglyTypedSerie
        {
            public string SerialNumber { get; set; }
            public double CpuTemp { get; set; }
        }
    }
}
