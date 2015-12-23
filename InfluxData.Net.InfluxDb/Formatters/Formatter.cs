using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.Formatters
{
    public class Formatter : IInfluxDbFormatter
    {
        private static readonly string _queryTemplate = "{0} {1} {2}"; // [key] [fields] [time]

        public virtual string GetLineTemplate()
        {
            return _queryTemplate;
        }

        /// <summary>
        /// Returns a point represented in line protocol format for writing to the InfluxDb API endpoint
        /// </summary>
        /// <returns>A string that represents this instance.</returns>
        /// <remarks>
        /// Example outputs:
        /// cpu,host=serverA,region=us_west value = 0.64
        /// payment,device=mobile,product=Notepad,method=credit billed = 33, licenses = 3i 1434067467100293230
        /// stock,symbol=AAPL bid = 127.46, ask = 127.48
        /// temperature,machine=unit42,type=assembly external = 25,internal=37 1434067467000000000
        /// </remarks>
        public virtual string PointToString(Point point)
        {
            Validate.NotNullOrEmpty(point.Name, "measurement");
            Validate.NotNull(point.Tags, "tags");
            Validate.NotNull(point.Fields, "fields");

            var tags = FormatPointTags(point.Tags);
            var fields = FormatPointFields(point.Fields);
            var key = FormatPointKey(point, tags);
            var timestamp = FormatPointTimestamp(point);

            var result = String.Format(GetLineTemplate(), key, fields, timestamp);

            return result;
        }

        public virtual Serie PointToSerie(Point point)
        {
            var serie = new Serie
            {
                Name = point.Name
            };

            foreach (var key in point.Tags.Keys.ToList())
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

        protected virtual string FormatPointTags(IDictionary<string, object> tags)
        {
            // NOTE: from InfluxDB documentation - "Tags should be sorted by key before being sent for best performance."
            return String.Join(",", tags.OrderBy(p => p.Key).Select(p => FormatPointTag(p.Key, p.Value)));
        }

        protected virtual string FormatPointTag(string key, object value)
        {
            return String.Join("=", key, EscapeTagValue(value.ToString()));
        }

        protected virtual string FormatPointFields(IDictionary<string, object> fields)
        {
            return String.Join(",", fields.Select(p => FormatPointField(p.Key, p.Value)));
        }

        protected virtual string FormatPointField(string key, object value)
        {
            Validate.NotNullOrEmpty(key, "key");
            Validate.NotNull(value, "value");

            var result = value.ToString();

            if (value.GetType() == typeof(string))
            {
                // Surround strings with quotes
                result = QuoteValue(EscapeNonTagValue(value.ToString()));
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

            return String.Join("=", EscapeNonTagValue(key), result);
        }

        protected virtual string FormatPointKey(Point point, string tags)
        {
            return String.IsNullOrEmpty(tags) ? EscapeNonTagValue(point.Name) : String.Join(",", EscapeNonTagValue(point.Name), tags);
        }

        protected virtual string FormatPointTimestamp(Point point)
        {
            return point.Timestamp.HasValue ? point.Timestamp.Value.ToUnixTime().ToString() : string.Empty;
        }

        protected virtual string ToInt(string result)
        {
            return result + "i";
        }

        protected virtual string EscapeNonTagValue(string value)
        {
            Validate.NotNull(value, "value");

            var result = value
                // literal backslash escaping is broken
                // https://github.com/influxdb/influxdb/issues/3070
                //.Replace(@"\", @"\\")
                .Replace(@"""", @"\""") // TODO: check if this is right or if "" should become \"\"
                .Replace(@" ", @"\ ")
                .Replace(@"=", @"\=")
                .Replace(@",", @"\,");

            return result;
        }

        protected virtual string EscapeTagValue(string value)
        {
            Validate.NotNull(value, "value");

            var result = value
                .Replace(@" ", @"\ ")
                .Replace(@"=", @"\=")
                .Replace(@",", @"\,");

            return result;
        }

        protected virtual string QuoteValue(string value)
        {
            Validate.NotNull(value, "value");

            return "\"" + value + "\"";
        }
    }
}