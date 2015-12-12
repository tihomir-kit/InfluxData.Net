using InfluxData.Net.Contracts;
using InfluxData.Net.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace InfluxData.Net.Models
{
    /// <summary>
    /// Represents an API continuous query object for CQ creation
    /// </summary>
    public class CqRequest
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
