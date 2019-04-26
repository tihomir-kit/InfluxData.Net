﻿using System;
using System.Collections.Generic;
using System.Linq;
using InfluxData.Net.Common.Constants;
using InfluxData.Net.InfluxDb.Formatters;

namespace InfluxData.Net.InfluxDb.Models
{
    /// <summary>
    /// Represents an InfluxDb write request.
    /// </summary>
    public class WriteRequest
    {
        private readonly IPointFormatter _formatter;

        public WriteRequest(IPointFormatter formatter)
        {
            _formatter = formatter;
        }

        /// <summary>
        /// Database name.
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// Points to write.
        /// </summary>
        public IEnumerable<Point> Points { get; set; }

        /// <summary>
        /// Point data time precision.
        /// </summary>
        public TimeUnit Precision { get; set; }

        /// <summary>
        /// Data retention policy.
        /// </summary>
        public string RetentionPolicy { get; set; }

        /// <summary>
        /// Gets the set of points in line protocol format.
        /// </summary>
        /// <returns></returns>
        public string GetLines()
        {
            return String.Join("\n", Points.Select(p => _formatter.PointToString(p, Precision)));
        }
    }
}
