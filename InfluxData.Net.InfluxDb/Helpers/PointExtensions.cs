using InfluxData.Net.Common.Attributes;
using InfluxData.Net.InfluxDb.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using InfluxData.Net.Common.Infrastructure;
using System.Reflection;

namespace InfluxData.Net.InfluxDb.Helpers
{
    public static class PointExtensions
    {
        /// <summary>
        /// Allows for converting attribute decorated types into a <see cref="Point"/> <para />
        /// Attribute rules: <para />
        /// 1) Must have exactly ONE [Measurement] attribute  <para />
        /// 2) Must not have more than ONE [Timestamp] attribute  <para />
        /// 3) Must have at least ONE [Field] attribute  <para />
        /// 4) [Tag] attribute is optional  <para />
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <example>
        /// Example of valid type:
        /// <code>
        /// 
        /// public class MyType
        /// {
        ///     [Measurement]
        ///     public string MyMeasurement { get; set; }
        ///     
        ///     [Timestamp]
        ///     public DateTime Time { get; set; }
        ///     
        ///     [Tag]
        ///     public string SignalName { get; set; }
        ///     
        ///     [Field]
        ///     public double Value { get; set; }
        /// }
        /// 
        /// </code>
        /// </example>
        public static Point ToPoint<TModel>(this TModel model)
        {
            var type = model.GetType();

            Point point = new Point
            {
                Fields = new Dictionary<string, object>(),
                Tags = new Dictionary<string, object>()
            };

            var properties = type.GetProperties();

            point.SetTimestamp(model, properties);
            point.SetMeasurement(model, properties);
            point.SetFields(model, properties);
            point.SetTags(model, properties);

            return point;
        }

        private static Point SetTimestamp<TModel>(this Point point, TModel model, PropertyInfo[] properties)
        {
            var timestampProperties = properties.Where(x => x.IsDefined(typeof(TimestampAttribute), false));

            // Make sure only one TimestampAttribute is defined
            if (timestampProperties.Any())
            {
                if (timestampProperties.Count() != 1)
                    throw new InvalidOperationException($"Cannot have multiple {typeof(TimestampAttribute).Name} attributes defined");

                var timestampProperty = timestampProperties.FirstOrDefault();
                var timestampPropertyValue = timestampProperty.GetValue(model);

                if (!timestampProperty.PropertyType.Equals(typeof(DateTime)))
                    throw new InvalidOperationException($"{nameof(timestampProperty.Name)} is not of type {typeof(DateTime).Name}");

                if (timestampPropertyValue == null)
                    throw new InvalidOperationException($"{nameof(timestampProperty.Name)} cannot be null");

                point.Timestamp = (DateTime)timestampPropertyValue;
            }

            return point;
        }

        private static Point SetMeasurement<TModel>(this Point point, TModel model, PropertyInfo[] properties)
        {
            var measurementProperties = properties.Where(x => x.IsDefined(typeof(MeasurementAttribute), false));

            if(!measurementProperties.Any())
            {
                throw new MissingExpectedAttributeException(typeof(MeasurementAttribute));
            }

            // Make sure only one MeasurementAttribute is defined
            if (measurementProperties.Count() != 1)
            {
                throw new InvalidOperationException($"Must have exactly one {typeof(MeasurementAttribute).Name} attribute defined");
            }

            // Make sure at least one FieldAttribute is defined
            if (!properties.Any(x => x.IsDefined(typeof(FieldAttribute), false)))
            {
                throw new MissingExpectedAttributeException(typeof(FieldAttribute));
            }

            var measurementProperty = measurementProperties.FirstOrDefault();
            var measurementPropertyValue = measurementProperty.GetValue(model);

            if (!measurementProperty.PropertyType.Equals(typeof(string)))
            {
                throw new InvalidOperationException($"{nameof(measurementProperty.Name)} is not of type {typeof(string).Name}");
            }

            if ((string.IsNullOrWhiteSpace((string)measurementPropertyValue)))
            {
                throw new InvalidOperationException($"{nameof(measurementProperty.Name)} cannot be null or whitespace");
            }

            point.Name = (string)measurementPropertyValue;

            return point;
        }

        private static Point SetTags<TModel>(this Point point, TModel model, PropertyInfo[] properties)
        {
            var tagProperties = properties.Where(x => x.IsDefined(typeof(TagAttribute), false));

            if (tagProperties.Any(x => !x.PropertyType.Equals(typeof(string))))
            {
                throw new InvalidOperationException($"Tags can only be string values");
            }

            foreach (var tagProperty in tagProperties)
            {
                var tagType = tagProperty.PropertyType;
                var tagValue = tagProperty.GetValue(model);

                if (tagValue == null)
                    continue;

                var converted = Convert.ChangeType(tagValue, tagType);

                var propertyName = tagProperty.GetCustomAttribute<TagAttribute>().Name;

                point.Tags.Add(propertyName, converted);
            }

            return point;
        }

        private static Point SetFields<TModel>(this Point point, TModel model, PropertyInfo[] properties)
        {
            var fieldProperties = properties.Where(x => x.IsDefined(typeof(FieldAttribute), false));

            if (fieldProperties.Any(x => !x.PropertyType.IsSimple()))
            {
                throw new InvalidOperationException($"Fields can only be primitive or string values");
            }

            foreach (var fieldProperty in fieldProperties)
            {
                var fieldType = fieldProperty.PropertyType;
                var fieldValue = fieldProperty.GetValue(model);

                if (fieldValue == null)
                    continue;

                var converted = Convert.ChangeType(fieldValue, fieldType);

                var propertyName = fieldProperty.GetCustomAttribute<FieldAttribute>().Name;

                point.Fields.Add(propertyName, converted);
            }

            return point;
        }

        private static bool IsSimple(this Type type)
        {
            return
                type.IsPrimitive ||
                type.Equals(typeof(String));
        }
    }
}
