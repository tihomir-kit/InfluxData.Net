using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.Common.Constants;

namespace InfluxData.Net.InfluxDb.Formatters
{
    public class PointFormatter : IPointFormatter
    {
        /// <summary>
        /// Returns a point represented in line protocol format for writing to the InfluxDb API endpoint
        /// </summary>
        /// <returns>A string that represents this instance.</returns>
        public virtual string PointToString(Point point, string precision = TimeUnit.Milliseconds)
        {
            Validate.IsNotNullOrEmpty(point.Name, "measurement");
            Validate.IsNotNull(point.Tags, "tags");
            Validate.IsNotNull(point.Fields, "fields");

            var tags = FormatPointTags(point.Tags);
            var fields = FormatPointFields(point.Fields);
            var key = FormatPointKey(point, tags); // key consists of measurement and tags
            var timestamp = FormatPointTimestamp(point, precision);

            var result = $"{key} {fields} {timestamp}";

            return result;
        }

        public virtual Serie PointToSerie(Point point)
        {
            var serie = new Serie
            {
                Name = point.Name
            };

            foreach (var key in point.Tags.Keys)
            {
                serie.Tags.Add(key, point.Tags[key].ToString());
            }

            var sortedFields = point.Fields.OrderBy(k => k.Key).ToDictionary(x => x.Key, x => x.Value);

            serie.Columns = new string[] { "time" }.Concat(sortedFields.Keys).ToArray();

            serie.Values = new object[][]
            {
                new object[] { point.Timestamp }.Concat(sortedFields.Values).ToArray()
            };

            return serie;
        }

        protected virtual string FormatPointKey(Point point, string tags)
        {
            var escapedPointKey = EscapeMeasurement(point.Name);
            return String.IsNullOrEmpty(tags) ? escapedPointKey : $"{escapedPointKey},{tags}";
        }

        /// <summary>
        /// For string field values use a backslash character \ to escape:
        ///   - double quotes "
        /// </summary>
        /// <param name="value">Value to escape.</param>
        /// <returns>Escaped value.</returns>
        protected virtual string EscapeMeasurement(string value)
        {
            Validate.IsNotNull(value, "value");

            var result = value
                .Replace(@",", @"\,")
                .Replace(@" ", @"\ ");

            return result;
        }

        protected virtual string FormatPointTags(IDictionary<string, object> tags)
        {
            // NOTE: from InfluxDB documentation - "Tags should be sorted by key before being sent for best performance."
            return tags.OrderBy(p => p.Key).Select(p => FormatPointTag(p.Key, p.Value)).ToCommaSeparatedString();
        }

        protected virtual string FormatPointTag(string key, object value)
        {
            return $"{EscapeTagOrKeyValue(key)}={EscapeTagOrKeyValue(value.ToString())}";
        }

        protected virtual string FormatPointFields(IDictionary<string, object> fields)
        {
            return fields.Select(p => FormatPointField(p.Key, p.Value)).ToCommaSeparatedString();
        }

        protected virtual string FormatPointField(string key, object value)
        {
            Validate.IsNotNullOrEmpty(key, "key");
            Validate.IsNotNull(value, "value");

            var result = value.ToString();

            if (value.GetType() == typeof(string))
            {
                // Surround strings with quotes
                result = QuoteFieldStringValue(value.ToString());
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

            return $"{EscapeTagOrKeyValue(key)}={result}";
        }

        protected virtual string FormatPointTimestamp(Point point, string precision = TimeUnit.Milliseconds)
        {
            return point.Timestamp.HasValue ? point.Timestamp.Value.ToUnixTime(precision).ToString() : string.Empty;
        }

        protected virtual string ToInt(string result)
        {
            return $"{result}i";
        }

        /// <summary>
        /// For tag keys, tag values, and field keys always use a backslash character \ to escape:
        ///   - commas ,
        ///   - equal signs =
        ///   - spaces
        /// </summary>
        /// <param name="value">Value to escape.</param>
        /// <returns>Escaped value.</returns>
        protected virtual string EscapeTagOrKeyValue(string value)
        {
            Validate.IsNotNull(value, "value");

            var result = value
                .Replace(@",", @"\,")
                .Replace(@"=", @"\=")
                .Replace(@" ", @"\ ");

            return result;
        }

        /// <summary>
        /// For string field values use a backslash character \ to escape:
        ///   - ouble quotes "
        /// </summary>
        /// <param name="value">Value to escape.</param>
        /// <returns>Escaped value.</returns>
        protected virtual string QuoteFieldStringValue(string value)
        {
            Validate.IsNotNull(value, "value");

            return $"\"{value}\"";
        }
    }
}