using System;
using System.Collections.Generic;

namespace InfluxData.Net.InfluxDb.Models.Responses
{
    /// <summary>
    /// Represents a generic query response.
    /// </summary>
    public class QueryResponse
    {
        /// <summary>
        /// Response error.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Query response represented as an IEnumerable of <see cref="{SeriesResult}"/> objects. 
        /// InfluxDb returns this kind of object by convention.
        /// </summary>
        /// <remarks>NOTE: DO NOT RENAME this property (used by convention to deserialize query response)</remarks>
        public IEnumerable<SeriesResult> Results { get; set; }
    }

    /// <summary>
    /// Represents series query result.
    /// </summary>
    public class SeriesResult
    {
        /// <summary>
        /// Serie result error.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// <see cref="IEnumerable{Serie}"/> result items.
        /// </summary>
        public IEnumerable<Serie> Series { get; set; }
    }
}