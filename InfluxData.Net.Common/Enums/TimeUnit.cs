using InfluxData.Net.Common.Helpers;

namespace InfluxData.Net.Common.Enums
{
    /// <summary>
    /// Time unit used in writing data points or parsing series.
    /// </summary>
    public enum TimeUnit
    {
        // NOTE: currently not supported
        //[ParamValue("n")]
        //Nanoseconds,

        // NOTE: currently not supported
        //[ParamValue("u")]
        //Microseconds,

        [ParamValue("ms")]
        Milliseconds,

        [ParamValue("s")]
        Seconds,

        [ParamValue("m")]
        Minutes,

        [ParamValue("h")]
        Hours
    }
}