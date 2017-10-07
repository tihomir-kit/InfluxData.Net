using System;
using System.Collections.Generic;
using System.Linq;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Models.Responses;
using System.Reflection;

namespace InfluxData.Net.InfluxDb.Helpers
{
    public static class SerieExtensions
    {
        /// <summary>
        /// Gets the byColumn value of the first Serie.Values record item. Usually for series that are
        /// known to return a single serie record-set (such as "SHOW DIAGNOSTICS" or "SHOW STATS").
        /// </summary>
        /// <typeparam name="T">Expected type of the byColumn value (int, long, string..).</typeparam>
        /// <param name="serie">Serie to parse.</param>
        /// <param name="columnKey">Column key of the needed value.</param>
        /// <returns>Parsed column value.</returns>
        public static T FirstRecordValueAs<T>(this Serie serie, string columnKey)
        {
            var firstValue = serie.Values.FirstOrDefault();
            if (firstValue == null)
                return default(T);

            return (T)(firstValue[serie.Columns.IndexOf(columnKey)]);
        }

        /// <summary>
        /// Converts an ubiquitous enumeration of series to a strongly typed enumeration by matching property names.
        /// </summary>
        /// <typeparam name="T">Type to convert the enumeration of series to</typeparam>
        /// <param name="series">Series to convert</param>
        /// <returns>Strongly typed enumeration representing the series</returns>
        public static IEnumerable<T> As<T>(this IEnumerable<Serie> series) where T : new()
        {
            if (series == null)
                yield return default(T);

            Type type = typeof(T);

            foreach (var serie in series)
            {
                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();

                var matchedProperties = serie.Columns
                    .Select(columnName => properties.FirstOrDefault(
                        property => String.Compare(property.Name, columnName, StringComparison.InvariantCultureIgnoreCase) == 0))
                    .ToList();

                foreach (var value in serie.Values)
                {
                    var instance = new T();

                    for (var columnIndex = 0; columnIndex < serie.Columns.Count(); columnIndex++)
                    {
                        var prop = matchedProperties[columnIndex];

                        if (prop == null)
                            continue;

                        Type propType = prop.PropertyType;

                        var convertedValue = Convert.ChangeType(value[columnIndex], prop.PropertyType);

                        prop.SetValue(instance, convertedValue);
                    }

                    yield return instance;
                }
            }
        }

        /// <summary>
        /// Extracts a serie object from a serie collection by serie/measurement name.
        /// </summary>
        /// <param name="series">Serie collection.</param>
        /// <param name="name">Serie/measurement name.</param>
        /// <returns>Serie item.</returns>
        public static Serie GetByName(this IEnumerable<Serie> series, string name)
        {
            var serie = series.FirstOrDefault(p => p.Name == name);
            Validate.IsNotNull(serie, String.Format("serie.GetByName('{0}')", name));

            return serie;
        }
    }
}
