using InfluxData.Net.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InfluxData.Net.InfluxDb.Models
{
    /// <summary>
    /// Represents a time series point for InfluxDb writes.
    /// <see cref="https://github.com/influxdb/influxdb/blob/master/tsdb/README.md" />
    /// </summary>
    public class Point
    {
        public Point()
        {
            Tags = new Dictionary<string, object>();
            Fields = new Dictionary<string, object>();
        }

        /// <summary>
        /// Measurement name (Serie name to write the data into).
        /// <see cref="https://influxdb.com/docs/v0.9/write_protocols/write_syntax.html"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Tags to write.
        /// </summary>
        public Dictionary<string, object> Tags { get; set; }

        /// <summary>
        /// Fields to write.
        /// </summary>
        public Dictionary<string, object> Fields { get; set; }

        /// <summary>
        /// Explicit point timestamp (optional).
        /// </summary>
        public DateTime? Timestamp { get; set; }
    }
}
