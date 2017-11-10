namespace InfluxData.Net.Common.Constants
{
    /// <summary>
    /// Time unit used in writing data points or parsing series.
    /// </summary>
    public static class TimeUnit
    {
        /// <summary>
        /// Use this value only with <see cref="Point2"/>
        /// </summary>
        public const string Nanoseconds = "ns";

        /// <summary>
        /// Use this value only with <see cref="Point2"/>
        /// </summary>
        public const string Microseconds = "u";

        public const string Milliseconds = "ms";

        public const string Seconds = "s";

        public const string Minutes = "m";

        public const string Hours = "h";
    }
}