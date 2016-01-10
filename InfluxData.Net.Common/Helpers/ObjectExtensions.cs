using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
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
        /// Converts DateTime to unix time (in milliseconds).
        /// </summary>
        /// <param name="date">DateTime to convert.</param>
        /// <returns>Unix-style timestamp in milliseconds.</returns>
        public static long ToUnixTime(this DateTime date)
        {
            return Convert.ToInt64((date - _epoch).TotalMilliseconds);
        }

        /// <summary>
        /// Converts from unix time (in milliseconds) to DateTime.
        /// </summary>
        /// <param name="unixTimeInMillis">The unix time in milliseconds.</param>
        /// <returns>DateTime object.</returns>
        public static DateTime FromUnixTime(this long unixTimeInMillis)
        {
            return _epoch.AddMilliseconds(unixTimeInMillis);
        }
    }
}
