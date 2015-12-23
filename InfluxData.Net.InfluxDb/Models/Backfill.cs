using System;
using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Enums;

namespace InfluxData.Net.InfluxDb.Models
{
    /// <summary>
    /// Represents a backfill object for manually backfilling (downsampling) aggregated series data.
    /// </summary>
    public class Backfill
    {
        /// <summary>
        /// InfluxDb downsampling functions/fields for the "SELECT" part of the query.
        /// </summary>
        public IList<string> Downsamplers { get; set; }

        /// <summary>
        /// Downsample serie name for the serie to write into.
        /// </summary>
        public string DsSerieName { get; set; }

        /// <summary>
        /// Source serie name.
        /// </summary>
        public string SourceSerieName { get; set; }

        /// <summary>
        /// Time from boundary.
        /// </summary>
        public DateTime TimeFrom { get; set; }

        /// <summary>
        /// Time to boundary.
        /// </summary>
        public DateTime TimeTo { get; set; }

        /// <summary>
        /// List of "WHERE" clause filters.
        /// </summary>
        public IList<string> Filters { get; set; }

        /// <summary>
        /// Data aggregation time interval.
        /// </summary>
        public string Interval { get; set; }

        /// <summary>
        /// Tags to retain in the destination serie.
        /// </summary>
        public IList<string> Tags { get; set; }

        /// <summary>
        /// <see cref="{FillType}"/> for time intervals with no data.
        /// <see cref="https://docs.influxdata.com/influxdb/v0.9/query_language/data_exploration/#the-group-by-clause-and-fill"/>
        /// </summary>
        public FillType FillType { get; set; }
    }
}
