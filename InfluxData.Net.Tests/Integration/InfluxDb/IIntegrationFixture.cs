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
using Ploeh.AutoFixture;
using InfluxData.Net.Common.Enums;

namespace InfluxData.Net.Integration.InfluxDb
{
    public interface IIntegrationFixture : IIntegrationFixtureBase
    {
        IInfluxDbClient Sut { get; set; }

        void Dispose();

        #region Validation

        /// <summary>
        /// Checks if the serie has expected point count in the database.
        /// </summary>
        /// <param name="serieName">Serie name to check.</param>
        /// <param name="countField">Point field to be used in 'count()' portion of the query.</param>
        /// <param name="expectedPoints">Expected number of saved points.</param>
        Task EnsureValidPointCount(string serieName, string countField, int expectedPoints);

        /// <summary>
        /// Checks if the point is in the database. (checks by serie name and timestamp).
        /// </summary>
        /// <param name="expectedPoint">Expected point.</param>
        Task EnsurePointExists(Point expectedPoint);

        #endregion Validation

        #region Data Mocks

        string CreateRandomMeasurementName();

        string CreateRandomCqName();

        /// <summary>
        /// Mocks a desired amount of points and saves them to the DB.
        /// </summary>
        /// <param name="amount">Amount per measurement to mock.</param>
        /// <param name="uniqueMeasurements">Unique measurements amount.</param>
        Task<IEnumerable<Point>> MockAndWritePoints(int amount, int uniqueMeasurements = 1, string dbName = null);

        /// <summary>
        /// Mocks a CQ and saves it to the DB.
        /// </summary>
        /// <param name="serieName">CQ for serie name?</param>
        Task<CqParams> MockAndWriteCq(string serieName);

        IEnumerable<Point> MockPoints(int amount);

        Dictionary<string, object> MockPointTags(Random rnd);

        Dictionary<string, object> MockPointFields(Random rnd);

        CqParams MockContinuousQuery(string serieName);

        BackfillParams MockBackfill();

        #endregion Data Mocks
    }
}