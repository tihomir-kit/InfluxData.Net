using InfluxData.Net.InfluxDb.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InfluxData.Net.InfluxDb.Models
{
    /// <summary>
    /// Represents an API continuous query object for CQ creation
    /// </summary>
    public class ContinuousQuery
    {
        public string DbName { get; set; }

        public string CqName { get; set; }

        public IList<string> Downsamplers { get; set; }

        public string DsSerieName { get; set; }

        public string SourceSerieName { get; set; }

        public string Interval { get; set; }

        public IList<string> Tags { get; set; }

        public FillType FillType { get; set; }
    }
}
