using FluentAssertions;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using Moq;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace InfluxData.Net.Integration
{
    // NOTE: http://stackoverflow.com/questions/106907/making-code-internal-but-available-for-unit-testing-from-other-projects

    public class IntegrationFixture : IDisposable
    {
        public static readonly string _fakeDbPrefix = "FakeDb";
        public static readonly string _fakeMeasurementPrefix = "FakeMeasurement";
        private MockRepository _mockRepository;

        public IInfluxDbClient Sut { get; set; }

        public string DbName { get; set; }

        public bool VerifyAll { get; set; }

        public IntegrationFixture()
        {
            InfluxDbVersion influxVersion;
            if (!Enum.TryParse(ConfigurationManager.AppSettings.Get("version"), out influxVersion))
                influxVersion = InfluxDbVersion.v096;

            this.Sut = new InfluxDb.InfluxDbClient(
                ConfigurationManager.AppSettings.Get("url"),
                ConfigurationManager.AppSettings.Get("username"),
                ConfigurationManager.AppSettings.Get("password"),
                influxVersion);

            this.Sut.Should().NotBeNull();

            this.DbName = CreateRandomDbName();

            Task.Run(() => this.PurgeFakeDatabases()).Wait();
            Task.Run(() => this.CreateEmptyDatabase()).Wait();
        }

        public void Dispose()
        {
            var deleteResponse = this.Sut.Database.DropDatabaseAsync(this.DbName).Result;

            deleteResponse.Success.Should().BeTrue();
        }

        // Per-test
        public void TestSetup()
        {
            _mockRepository = new MockRepository(MockBehavior.Strict);
            VerifyAll = true;
        }

        // Per-test
        public void TestTearDown()
        {
            if (VerifyAll)
            {
                _mockRepository.VerifyAll();
            }
            else
            {
                _mockRepository.Verify();
            }
        }


        private async Task CreateEmptyDatabase()
        {
            var createResponse = await this.Sut.Database.CreateDatabaseAsync(this.DbName);
            createResponse.Success.Should().BeTrue();
        }

        private async Task PurgeFakeDatabases()
        {
            var dbs = await this.Sut.Database.ShowDatabasesAsync();

            foreach (var db in dbs)
            {
                if (db.Name.StartsWith(_fakeDbPrefix))
                    await this.Sut.Database.DropDatabaseAsync(db.Name);
            }
        }

        public string CreateRandomDbName()
        {
            var timestamp = DateTime.UtcNow.ToUnixTime();
            return String.Format("{0}{1}", _fakeDbPrefix, timestamp);
        }

        public string CreateRandomMeasurementName()
        {
            var timestamp = DateTime.UtcNow.ToUnixTime();
            return String.Format("{0}{1}", _fakeMeasurementPrefix, timestamp);
        }

        public async Task<List<Serie>> Query(Serie expected)
        {
            // 0.9.3 need 'group by' to retrieve tags as tags when using select *
            var result = await this.Sut.Client.QueryAsync(this.DbName, String.Format("select * from \"{0}\" group by *", expected.Name));

            result.Should().NotBeNull();
            result.Count().Should().Be(1);

            var actual = result.Single();

            actual.Name.Should().Be(expected.Name);
            actual.Tags.Count.Should().Be(expected.Tags.Count);
            actual.Tags.ShouldAllBeEquivalentTo(expected.Tags);
            actual.Columns.ShouldAllBeEquivalentTo(expected.Columns);
            actual.Columns.Count().Should().Be(expected.Columns.Count());
            actual.Values[0].Count().Should().Be(expected.Values[0].Count());
            ((DateTime)actual.Values[0][0]).ToUnixTime().Should().Be(((DateTime)expected.Values[0][0]).ToUnixTime());

            return result;
        }

        public Point[] CreateMockPoints(int amount)
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

        public Dictionary<string, object> CreateNewTags(Random rnd)
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

        public Dictionary<string, object> CreateNewFields(Random rnd)
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

        public ContinuousQuery MockContinuousQuery()
        {
            return new ContinuousQuery()
            {
                DbName = this.DbName,
                CqName = "FakeCQ",
                Downsamplers = new List<string>()
                {
                    "MAX(field_int) AS max_field_int",
                    "MIN(field_int) AS min_field_int"
                },
                DsSerieName = String.Format("{0}.5s", _fakeMeasurementPrefix),
                SourceSerieName = _fakeMeasurementPrefix,
                Interval = "5s",
                FillType = FillType.Previous
            };
        }

        public Backfill MockBackfill()
        {
            return new Backfill()
            {
                Downsamplers = new List<string>()
                {
                    "MAX(field_int) AS max_field_int",
                    "MIN(field_int) AS min_field_int"
                },
                DsSerieName = String.Format("{0}.5m", _fakeMeasurementPrefix),
                SourceSerieName = _fakeMeasurementPrefix,
                TimeFrom = DateTime.UtcNow.AddMonths(-1),
                TimeTo = DateTime.UtcNow,
                Interval = "5m",
                FillType = FillType.None
            };
        }
    }
}