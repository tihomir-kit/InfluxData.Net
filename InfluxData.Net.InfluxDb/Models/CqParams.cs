using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Enums;
using System;

namespace InfluxData.Net.InfluxDb.Models
{
    /// <summary>
    /// Represents a continuous query object that describes a CQ to create.
    /// </summary>
    public class CqParams
    {
        /// <summary>
        /// Database name.
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// New continuous query name.
        /// </summary>
        public string CqName { get; set; }

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
