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
        public static readonly string _fakeCq = "FakeCq";

        private static readonly Random _random = new Random();
        private static readonly object _syncLock = new object();
        private MockRepository _mockRepository;

        public IInfluxDbClient Sut { get; set; }

        public string DbName { get; set; }

        public bool VerifyAll { get; set; }

        public IntegrationFixture()
        {
            InfluxDbVersion influxVersion;
            if (!Enum.TryParse(ConfigurationManager.AppSettings.Get("version"), out influxVersion))
                influxVersion = InfluxDbVersion.v_0_9_6;

            this.Sut = new InfluxDb.InfluxDbClient(
                ConfigurationManager.AppSettings.Get("endpointUri"),
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


        public async Task CreateEmptyDatabase(string dbName = null)
        {
            var createResponse = await this.Sut.Database.CreateDatabaseAsync(dbName ?? this.DbName);
            createResponse.Success.Should().BeTrue();
        }

        private async Task PurgeFakeDatabases()
        {
            var dbs = await this.Sut.Database.GetDatabasesAsync();

            foreach (var db in dbs)
            {
                if (db.Name.StartsWith(_fakeDbPrefix))
                    await this.Sut.Database.DropDatabaseAsync(db.Name);
            }
        }

        public string CreateRandomDbName()
        {
            return String.Format("{0}{1}", _fakeDbPrefix, CreateRandomSuffix());
        }

        public string CreateRandomMeasurementName()
        {
            return String.Format("{0}{1}", _fakeMeasurementPrefix, CreateRandomSuffix());
        }

        public string CreateRandomCqName()
        {
            return String.Format("{0}{1}", _fakeCq, CreateRandomSuffix());
        }

        /// <see cref="http://stackoverflow.com/a/768001/413785"/>
        public static string CreateRandomSuffix()
        {
            var timestamp = DateTime.UtcNow.ToUnixTime();
            lock (_syncLock)
            {
                var randomInt = _random.Next(Int32.MaxValue);
                return String.Format("{0}{1}", timestamp, randomInt);
            }
        }

        /// <summary>
        /// Checks if the serie has expected point count.
        /// </summary>
        /// <param name="serieName">Serie name to check.</param>
        /// <param name="countField">Point field to be used in 'count()' portion of the query.</param>
        /// <param name="expectedPoints">Expected number of saved points.</param>
        public async Task EnsureValidPointCount(string serieName, string countField, int expectedPoints)
        {
            var response = await this.Sut.Client.QueryAsync(this.DbName, String.Format("select count({0}) from \"{1}\"", countField, serieName));
            response.Should().NotBeNull();
            response.Count().Should().Be(1);
            var countIndex = Array.IndexOf(response.First().Columns.ToArray(), "count");
            response.First().Values.First()[countIndex].ToString().Should().Be(expectedPoints.ToString());
        }

        /// <summary>
        /// Checks if the point is in the database. (checks by serie name and timestamp).
        /// </summary>
        /// <param name="expectedPoint">Expected point.</param>
        public async Task EnsurePointExists(Point expectedPoint)
        {
            var expectedSerie = this.Sut.GetFormatter().PointToSerie(expectedPoint);

            var response = await this.Sut.Client.QueryAsync(this.DbName, String.Format("select * from \"{0}\" group by * order by time desc", expectedPoint.Name));
            response.Should().NotBeNull();
            response.Count().Should().BeGreaterOrEqualTo(1);

            var serie = response.FirstOrDefault(p => ((DateTime)p.Values[0][0]).ToUnixTime() == ((DateTime)expectedPoint.Timestamp).ToUnixTime());
            serie.Should().NotBeNull();
            serie.Name.Should().Be(expectedSerie.Name);
            serie.Tags.Count.Should().Be(expectedSerie.Tags.Count);
            serie.Tags.ShouldAllBeEquivalentTo(expectedSerie.Tags);
            serie.Columns.ShouldAllBeEquivalentTo(expectedSerie.Columns);
            serie.Columns.Count().Should().Be(expectedSerie.Columns.Count());
            serie.Values[0].Count().Should().Be(expectedSerie.Values[0].Count());
        }

        /// <summary>
        /// Mocks a desired amount of points and saves them to the DB.
        /// </summary>
        /// <param name="amount">Amount per measurement to mock.</param>
        /// <param name="uniqueMeasurements">Unique measurements amount.</param>
        public async Task<IEnumerable<Point>> MockAndWritePoints(int amount, int uniqueMeasurements = 1)
        {
            var points = new Point[0];

            for (var i = 0; i < uniqueMeasurements; i++)
            {
                points = points.Concat(MockPoints(amount)).ToArray();
            }

            var writeResponse = await Sut.Client.WriteAsync(this.DbName, points.ToArray());
            writeResponse.Success.Should().BeTrue();

            return points;
        }

        /// <summary>
        /// Mocks a CQ and saves it to the DB.
        /// </summary>
        /// <param name="serieName">CQ for serie name?</param>
        public async Task<CqParams> MockAndWriteCq(string serieName)
        {
            var cq = MockContinuousQuery(serieName);
            var result = await Sut.ContinuousQuery.CreateContinuousQueryAsync(cq);
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();

            return cq;
        }

        public IEnumerable<Point> MockPoints(int amount)
        {
            var rnd = new Random();
            var fixture = new Fixture();

            fixture.Customize<Point>(c => c
                .With(p => p.Name, CreateRandomMeasurementName())
                .Do(p => p.Tags = MockPointTags(rnd))
                .Do(p => p.Fields = MockPointFields(rnd))
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

        public Dictionary<string, object> MockPointTags(Random rnd)
        {
            return new Dictionary<string, object>
            {
                // quotes in the tag value are creating problems
                // https://github.com/influxdb/influxdb/issues/3928
                //{"tag_string", rnd.NextPrintableString(50).Replace("\"", string.Empty)},
                { "tag_bool", (rnd.Next(2) == 0).ToString() },
                { "tag_datetime", DateTime.Now.ToString() },
                { "tag_decimal", ((decimal) rnd.NextDouble()).ToString() },
                { "tag_float", ((float) rnd.NextDouble()).ToString() },
                { "tag_int", rnd.Next().ToString() }
            };
        }

        public Dictionary<string, object> MockPointFields(Random rnd)
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

        public CqParams MockContinuousQuery(string serieName)
        {
            return new CqParams()
            {
                DbName = this.DbName,
                CqName = CreateRandomCqName(),
                Downsamplers = new List<string>()
                {
                    "MAX(field_int) AS max_field_int",
                    "MIN(field_int) AS min_field_int"
                },
                DsSerieName = String.Format("{0}.5s", serieName),
                SourceSerieName = serieName,
                Interval = "5s",
                FillType = FillType.Previous
            };
        }

        public BackfillParams MockBackfill()
        {
            return new BackfillParams()
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