using InfluxData.Net.Enums;
using System;

namespace InfluxData.Net.Helpers
{
    internal static class TimeUnitUtility
    {
        public static string ToTimePrecision(TimeUnit timeUnit)
        {
            switch (timeUnit)
            {
                case TimeUnit.Seconds:
                    return "s";
                case TimeUnit.Milliseconds:
                    return "ms";
                case TimeUnit.Microseconds:
                    return "u";
                default:
                    throw new ArgumentException("Time precision must be " + TimeUnit.Seconds + ", " + TimeUnit.Milliseconds + " or " + TimeUnit.Microseconds);
            }
        }
    }
}