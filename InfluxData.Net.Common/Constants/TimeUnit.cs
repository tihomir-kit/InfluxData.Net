namespace InfluxData.Net.Common.Constants
{
    /// <summary>
    /// Time unit used in writing data points or parsing series.
    /// </summary>
    /// 

    public enum TimeUnit
    {
        Nanoseconds = 0,
        Ticks = 1,
        Microseconds = 2,
        Milliseconds = 3,
        Seconds = 4,
        Minutes = 5,
        Hours = 6
    }

    public static class TimeUnitExtensions
    {
        public static string ToEpochFormat(this TimeUnit tu)
        {
            switch (tu)
            {
                case TimeUnit.Nanoseconds:
                    return "ns";
                case TimeUnit.Ticks:
                    return "ns";
                case TimeUnit.Microseconds:
                    return "u";
                case TimeUnit.Milliseconds:
                    return "ms";
                case TimeUnit.Seconds:
                    return "s";
                case TimeUnit.Minutes:
                    return "h";
                case TimeUnit.Hours:
                    return "h";
                default:
                    return "ns";
            }
        }
    }

    //public static class TimeUnit
    //{
    //    // NOTE: currently not supported
    //    //public const string Nanoseconds = "n";

    //    // NOTE: currently not supported
    //    //public const string Microseconds = "u";

    //    public const string Ticks = "ticks";

    //    public const string Milliseconds = "ms";

    //    public const string Seconds = "s";

    //    public const string Minutes = "m";

    //    public const string Hours = "h";
    //}
}