using InfluxData.Net.Common.Constants;
using InfluxData.Net.Common.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace InfluxData.Net.Common.Helpers
{
    /// <summary>
    /// Helper object extensions.
    /// </summary>
    public static class ObjectExtensions
    {
        private static DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Converts objects to JSON (for debugging purposes).
        /// </summary>
        /// <param name="object">Object to convert.</param>
        /// <returns>Object as JSON string.</returns>
        public static string ToJson(this object @object)
        {
            return JsonConvert.SerializeObject(@object);
        }

        /// <summary>
        /// Converts <see cref="{TimeUnit}"/> enum to param InfluxDb-ready string value.
        /// </summary>
        /// <param name="value"><see cref="{TimeUnit}"/> enum.</param>
        /// <returns>InfluxDb-ready time unit string value.</returns>
        public static string GetParamValue(this Enum value)
        {
            var valueFields = value.GetType().GetField(value.ToString());
            var paramValue = (ParamValueAttribute[])(valueFields)
                        .GetCustomAttributes(typeof(ParamValueAttribute), false);

            return paramValue.Length > 0 ? paramValue[0].Value : value.ToString();
        }

        /// <summary>
        /// Converts DateTime to unix time (default to milliseconds).
        /// </summary>
        /// <param name="date">DateTime to convert.</param>
        /// <param name="precision">Precision (optional, defaults to milliseconds)</param>
        /// <returns>Unix-style timestamp in milliseconds.</returns>
        public static long ToUnixTime(this DateTime date, string precision = TimeUnit.Milliseconds)
        {
            var span = date - _epoch;
            double fractionalSpan;

            switch (precision)
            {
                case TimeUnit.Milliseconds:
                    fractionalSpan = span.TotalMilliseconds;
                    break;
                case TimeUnit.Seconds:
                    fractionalSpan = span.TotalSeconds;
                    break;
                case TimeUnit.Minutes:
                    fractionalSpan = span.TotalMinutes;
                    break;
                case TimeUnit.Hours:
                    fractionalSpan = span.TotalHours;
                    break;
                default:
                    fractionalSpan = span.TotalMilliseconds;
                    break;
            }

            return Convert.ToInt64(fractionalSpan);
        }

        /// <summary>
        /// Converts from unix time (in milliseconds) to DateTime.
        /// </summary>
        /// <param name="unixTime">The unix time (milliseconds by default).</param>
        /// <param name="precision">Precision (optional, defaults to milliseconds)</param>
        /// <returns>DateTime object.</returns>
        public static DateTime FromUnixTime(this long unixTime, string precision = TimeUnit.Milliseconds)
        {
            switch (precision)
            {
                case TimeUnit.Milliseconds:
                    return _epoch.AddMilliseconds(unixTime);
                case TimeUnit.Seconds:
                    return _epoch.AddSeconds(unixTime);
                case TimeUnit.Minutes:
                    return _epoch.AddMinutes(unixTime);
                case TimeUnit.Hours:
                    return _epoch.AddHours(unixTime);
                default:
                    return _epoch.AddMilliseconds(unixTime);
            }
        }

        /// <summary>
        /// Joins items separating them with "," (comma).
        /// </summary>
        /// <param name="items">Items to join.</param>
        /// <returns>Comma separated collection as a string.</returns>
        public static string ToCommaSeparatedString(this IEnumerable<string> items)
        {
            return String.Join(",", items);
        }

        /// <summary>
        /// Joins items separating them with ", " (comma and one whitespace).
        /// </summary>
        /// <param name="items">Items to join.</param>
        /// <returns>Comma-space separated collection as a string.</returns>
        public static string ToCommaSpaceSeparatedString(this IEnumerable<string> items)
        {
            return String.Join(", ", items);
        }

        /// <summary>
        /// Joins items separating them with "AND " ('AND' and one whitespace).
        /// </summary>
        /// <param name="items">Items to join.</param>
        /// <returns>AND-space separated collection as a string.</returns>
        public static string ToAndSpaceSeparatedString(this IEnumerable<string> items)
        {
            return String.Join("AND ", items);
        }

        /// <summary>
        /// Joins items separating them with "; " (';' and one whitespace).
        /// </summary>
        /// <param name="items">Items to join.</param>
        /// <returns>Semicolon-space separated collection as a string.</returns>
        public static string ToSemicolonSpaceSeparatedString(this IEnumerable<string> items)
        {
            return String.Join("; ", items);
        }
    }
}
