using InfluxData.Net.InfluxDb.Formatters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InfluxData.Net.InfluxDb.Models
{
    /// <summary>
    /// Represents an API write request
    /// </summary>
    public class WriteRequest
    {
        private readonly IInfluxDbFormatter _formatter;

        public WriteRequest(IInfluxDbFormatter formatter)
        {
            _formatter = formatter;
        }

        public string Database { get; set; }

        public string RetentionPolicy { get; set; }

        public Point[] Points { get; set; }

        /// <summary>Gets the set of points in line protocol format.</summary>
        /// <returns></returns>
        public string GetLines()
        {
            return String.Join("\n", Points.Select(p => _formatter.PointToString(p)));
        }
    }
}
