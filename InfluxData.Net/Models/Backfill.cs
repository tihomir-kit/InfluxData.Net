using InfluxData.Net.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InfluxData.Net.Models
{
    /// <summary>
    /// Represents an API backfill object for manually backfilling old downsample series
    /// </summary>
    public class Backfill
    {
        public IList<string> Downsamplers { get; set; }

        public string DsSerieName { get; set; }

        public string SourceSerieName { get; set; }

        public DateTime TimeFrom { get; set; }

        public DateTime TimeTo { get; set; }

        public IList<string> Filters { get; set; }

        public string Interval { get; set; }

        public IList<string> Tags { get; set; }

        public FillType FillType { get; set; }
    }
}
