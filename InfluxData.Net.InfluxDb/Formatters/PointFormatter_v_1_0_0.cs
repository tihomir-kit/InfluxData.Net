using InfluxData.Net.Common.Constants;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace InfluxData.Net.InfluxDb.Formatters
{
    public class PointFormatter_v_1_0_0 : PointFormatter
    {
        protected override string FormatPointTag(string key, object value)
        {
            return $"{key}={EscapeTagValue(value.ToString())}";
        }

        protected override string FormatPointField(string key, object value)
        {
            Validate.IsNotNullOrEmpty(key, "key");
            Validate.IsNotNull(value, "value");

            var result = value.ToString();

            if (value.GetType() == typeof(string))
            {
                // Surround strings with quotes
                result = QuoteFieldStringValue(EscapeNonTagValue(value.ToString()));
            }
            else if (value.GetType() == typeof(bool))
            {
                // API needs lowercase booleans
                result = value.ToString().ToLower();
            }
            else if (value.GetType() == typeof(DateTime))
            {
                // InfluxDb does not support a datetime type for fields or tags
                // Convert datetime to UNIX long
                result = ((DateTime)value).ToUnixTime().ToString();
            }
            else if (value.GetType() == typeof(decimal))
            {
                // For cultures using other decimal characters than '.'
                result = ((decimal)value).ToString("0.0###################", CultureInfo.InvariantCulture);
            }
            else if (value.GetType() == typeof(float))
            {
                result = ((float)value).ToString("0.0###################", CultureInfo.InvariantCulture);
            }
            else if (value.GetType() == typeof(double))
            {
                result = ((double)value).ToString("0.0###################", CultureInfo.InvariantCulture);
            }
            else if (value.GetType() == typeof(long) || value.GetType() == typeof(int))
            {
                // Int requires 'i' at the end of the number - otherwise it will be represented as float
                result = ToInt(result);
            }

            return $"{EscapeNonTagValue(key)}={result}";
        }

        protected override string FormatPointKey(Point point, string tags)
        {
            var escapedPointKey = EscapeNonTagValue(point.Name);
            return String.IsNullOrEmpty(tags) ? escapedPointKey : $"{escapedPointKey},{tags}";
        }

        protected virtual string EscapeTagValue(string value)
        {
            Validate.IsNotNull(value, "value");

            var result = value
                .Replace(@" ", @"\ ")
                .Replace(@"=", @"\=")
                .Replace(@",", @"\,");

            return result;
        }

        protected virtual string EscapeNonTagValue(string value)
        {
            Validate.IsNotNull(value, "value");

            var result = value
                // literal backslash escaping is broken
                // https://github.com/influxdb/influxdb/issues/3070
                .Replace(@"\", @"\\")
                .Replace(@"""", @"\""")
                .Replace(@" ", @"\ ")
                .Replace(@"=", @"\=")
                .Replace(@",", @"\,");

            return result;
        }
    }
}