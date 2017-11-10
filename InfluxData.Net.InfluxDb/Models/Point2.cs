using System;
using System.Collections.Generic;
using System.Text;

namespace InfluxData.Net.InfluxDb.Models
{
    /// <summary>
    /// Special case designed to handle UnixTimeStamp and nano seconds
    /// Use this class to provide time in micro or nano seconds
    /// </summary>
    public class Point2 : Point
    {
        /// <summary>
        /// Unix timestamp. Could be in nano seconds
        /// If you are using Point2, then UnixTimeStamp will be used and TimeStamp will be ignored
        /// </summary>
        public long? UnixTimeStamp { get; set; }
    }
}
