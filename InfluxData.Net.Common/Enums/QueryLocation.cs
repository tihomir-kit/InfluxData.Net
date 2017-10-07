namespace InfluxData.Net.Common.Enums
{
    /// <summary>
    /// Allows long queries to be sent through form data rather than URI params.
    /// <see cref="https://github.com/influxdata/influxdb/commit/f58a50c231afd6ee775ce213461c7c0043cd724f"/>
    /// </summary>
    public enum QueryLocation
    {
        /// <summary>
        /// Send query data as URI params.
        /// </summary>
        Uri,

        /// <summary>
        /// Send query data as Multipart form data content.
        /// </summary>
        FormData
    }
}
