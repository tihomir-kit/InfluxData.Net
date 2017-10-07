using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.Integration.Kapacitor;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Constants;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.Integration.InfluxDb
{
    public abstract class IntegrationFixtureBase : IntegrationFixtureFactory, IIntegrationFixture
    {
        public static readonly string _fakeMeasurementPrefix = "FakeMeasurement";
        public static readonly string _fakeCq = "FakeCq";

        public IInfluxDbClient Sut { get; set; }

        protected IntegrationFixtureBase(string influxDbEndpointUriKey, InfluxDbVersion influxDbVersion, bool throwOnWarning = true) 
            : base("FakeInfluxDb", influxDbEndpointUriKey, influxDbVersion, throwOnWarning)
        {
            this.Sut = base.InfluxDbClient;
            this.Sut.Should().NotBeNull();
        }

        public void Dispose()
        {
        }

        #region Validation

        /// <summary>
        /// Checks if the serie has expected point count in the database.
        /// </summary>
        /// <param name="serieName">Serie name to check.</param>
        /// <param name="countField">Point field to be used in 'count()' portion of the query.</param>
        /// <param name="expectedPoints">Expected number of saved points.</param>
        public async Task EnsureValidPointCount(string serieName, string countField, int expectedPoints)
        {
            var response = await this.Sut.Client.QueryAsync(String.Format("select count({0}) from \"{1}\"", countField, serieName), this.DbName);
            response.Should().NotBeNull();
            response.Count().Should().Be(1);
            var countIndex = Array.IndexOf(response.First().Columns.ToArray(), "count");
            response.First().Values.First()[countIndex].ToString().Should().Be(expectedPoints.ToString());
        }

        /// <summary>
        /// Checks if the point is in the database. (checks by serie name and timestamp).
        /// </summary>
        /// <param name="expectedPoint">Expected point.</param>
        /// <param name="precision">Precision (optional, defaults to milliseconds)</param>
        /// <returns>Task with the expected serie.</returns>
        public async Task<Serie> EnsurePointExists(Point expectedPoint, string precision = TimeUnit.Milliseconds)
        {
            var expectedSerie = this.Sut.RequestClient.GetPointFormatter().PointToSerie(expectedPoint);

            var response = await this.Sut.Client.QueryAsync(String.Format("select * from \"{0}\" group by * order by time desc", expectedPoint.Name), this.DbName);
            response.Should().NotBeNull();
            response.Count().Should().BeGreaterOrEqualTo(1);

            var serie = response.FirstOrDefault(p => ((DateTime)p.Values[0][0]).ToUnixTime(precision) == ((DateTime)expectedPoint.Timestamp).ToUnixTime(precision));
            serie.Should().NotBeNull();
            serie.Name.Should().Be(expectedSerie.Name);
            serie.Tags.Count.Should().Be(expectedSerie.Tags.Count);
            serie.Tags.ShouldAllBeEquivalentTo(expectedSerie.Tags);
            serie.Columns.ShouldAllBeEquivalentTo(expectedSerie.Columns);
            serie.Columns.Count().Should().Be(expectedSerie.Columns.Count());
            serie.Values[0].Count().Should().Be(expectedSerie.Values[0].Count());

            return serie;
        }

        #endregion Validation

        #region Data Mocks

        public string CreateRandomMeasurementName()
        {
            return CreateRandomName(_fakeMeasurementPrefix);
        }

        public string CreateRandomCqName()
        {
            return CreateRandomName(_fakeCq);
        }

        /// <summary>
        /// Mocks a desired amount of points and saves them to the DB.
        /// </summary>
        /// <param name="amount">Amount per measurement to mock.</param>
        /// <param name="uniqueMeasurements">Unique measurements amount.</param>
        public async Task<IEnumerable<Point>> MockAndWritePoints(int amount, int uniqueMeasurements = 1, string dbName = null)
        {
            var points = new Point[0];

            for (var i = 0; i < uniqueMeasurements; i++)
            {
                points = points.Concat(MockPoints(amount)).ToArray();
            }

            var writeResponse = await Sut.Client.WriteAsync(points.ToArray(), dbName ?? this.DbName);
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
            // TODO: code below commented because it relies on AutoFixture, 
            // which is not dotnet - core compatible(yet), and has therefore
            // been replaced with a "poor man's" variant on this.

            var response = new List<Point>();
            var rnd = new Random();
            var timestamp = DateTime.UtcNow.AddDays(-5);

            var measurementName = CreateRandomMeasurementName();
            for (var i = 0; i < amount; i++)
            {
                timestamp = timestamp.AddMinutes(1);

                var p = new Point()
                {
                    Name = measurementName,
                    Fields = MockPointFields(rnd),
                    Tags = MockPointTags(rnd),
                    Timestamp = timestamp
                };
                response.Add(p);
            }

            return response;

            //var fixture = new Fixture();

            //fixture.Customize<Point>(c => c
            //    .With(p => p.Name, CreateRandomMeasurementName())
            //    .Do(p => p.Tags = MockPointTags(rnd))
            //    .Do(p => p.Fields = MockPointFields(rnd))
            //    .OmitAutoProperties());

            //var points = fixture.CreateMany<Point>(amount).ToArray();
            //var timestamp = DateTime.UtcNow.AddDays(-5);
            //foreach (var point in points)
            //{
            //    timestamp = timestamp.AddMinutes(1);
            //    point.Timestamp = timestamp;
            //}

            //return points;
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

        #endregion Data Mocks
    }
}