using FluentAssertions;
using InfluxData.Net.Enums;
using InfluxData.Net.Helpers;
using InfluxData.Net.Models;
using Moq;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace InfluxData.Net.Tests
{
    // NOTE: http://stackoverflow.com/questions/106907/making-code-internal-but-available-for-unit-testing-from-other-projects

    //[TestFixture]
    public class InfluxDbIntegrationTestsBase
    {
        protected IInfluxDb _influx;
        protected string _dbName = String.Empty;
        protected static readonly string _fakeDbPrefix = "FakeDb";
        protected static readonly string _fakeMeasurementPrefix = "FakeMeasurement";

        private MockRepository _mockRepository;
        protected bool VerifyAll { get; set; }


        //[SetUp]
        public void Setup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            VerifyAll = true;

            FinalizeSetUp();
        }

        //[TearDown]
        public void TearDown()
        {
            if (VerifyAll)
            { 
                _mockRepository.VerifyAll();
            }
            else
            { 
                _mockRepository.Verify();
            }

            FinalizeTearDown();
        }

        //[TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            InfluxVersion influxVersion;
            if (!Enum.TryParse(ConfigurationManager.AppSettings.Get("version"), out influxVersion))
                influxVersion = InfluxVersion.v096;

            _influx = new InfluxDb(
                ConfigurationManager.AppSettings.Get("url"),
                ConfigurationManager.AppSettings.Get("username"),
                ConfigurationManager.AppSettings.Get("password"),
                influxVersion);

            _influx.Should().NotBeNull();

            _dbName = CreateRandomDbName();

            PurgeFakeDatabases();

            var createResponse = _influx.CreateDatabaseAsync(_dbName).Result;
            createResponse.Success.Should().BeTrue();

            // workaround for issue https://github.com/influxdb/influxdb/issues/3363
            // by first creating a single point in the empty db
            var writeResponse = _influx.WriteAsync(_dbName, CreateMockPoints(1));
            writeResponse.Result.Success.Should().BeTrue();
        }

        //[TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            var deleteResponse = _influx.DropDatabaseAsync(_dbName).Result;

            deleteResponse.Success.Should().BeTrue();
        }
        

        protected virtual void FinalizeTearDown()
        {
        }

        protected virtual void FinalizeSetUp()
        {
        }

        private async Task PurgeFakeDatabases()
        {
            var dbs = await _influx.ShowDatabasesAsync();

            foreach (var db in dbs)
            {
                if (db.Name.StartsWith(_fakeDbPrefix))
                    await _influx.DropDatabaseAsync(db.Name);
            }
        }

        protected static string CreateRandomDbName()
        {
            var timestamp = DateTime.UtcNow.ToUnixTime();
            return String.Format("{0}{1}", _fakeDbPrefix, timestamp);
        }

        protected static string CreateRandomMeasurementName()
        {
            var timestamp = DateTime.UtcNow.ToUnixTime();
            return String.Format("{0}{1}", _fakeMeasurementPrefix, timestamp);
        }

        protected Point[] CreateMockPoints(int amount)
        {
            var rnd = new Random();
            var fixture = new Fixture();

            fixture.Customize<Point>(c => c
                .With(p => p.Name, CreateRandomMeasurementName())
                .Do(p => p.Tags = CreateNewTags(rnd))
                .Do(p => p.Fields = CreateNewFields(rnd))
                .OmitAutoProperties());

            var points = fixture.CreateMany<Point>(amount).ToArray();
            var timestamp = DateTime.UtcNow.AddDays(-5);
            foreach (var point in points)
            {
                timestamp = timestamp.AddMinutes(1);
                point.Timestamp = timestamp;
            }

            return points;
        }

        protected Dictionary<string, object> CreateNewTags(Random rnd)
        {
            return new Dictionary<string, object>
            {
                // quotes in the tag value are creating problems
                // https://github.com/influxdb/influxdb/issues/3928
                //{"tag_string", rnd.NextPrintableString(50).Replace("\"", string.Empty)},
                {"tag_bool", (rnd.Next(2) == 0).ToString()},
                {"tag_datetime", DateTime.Now.ToString()},
                {"tag_decimal", ((decimal) rnd.NextDouble()).ToString()},
                {"tag_float", ((float) rnd.NextDouble()).ToString()},
                {"tag_int", rnd.Next().ToString()}
            };
        }

        protected Dictionary<string, object> CreateNewFields(Random rnd)
        {
            return new Dictionary<string, object>
            {
                //{ "field_string", rnd.NextPrintableString(50) },
                { "field_bool", rnd.Next(2) == 0 },
                { "field_int", rnd.Next() },
                { "field_decimal", (decimal)rnd.NextDouble() },
                { "field_float", (float)rnd.NextDouble() },
                { "field_datetime", DateTime.Now }
            };
        }

        protected ContinuousQuery MockContinuousQuery()
        {
            return new ContinuousQuery()
            {
                DbName = _dbName,
                CqName = "FakeCQ",
                Downsamplers = new List<string>() { "AVG(field_int)" },
                DsSerieName = String.Format("{0}.5s", _fakeMeasurementPrefix),
                SourceSerieName = _fakeMeasurementPrefix,
                Interval = "5s"
            };
        }
    }
}