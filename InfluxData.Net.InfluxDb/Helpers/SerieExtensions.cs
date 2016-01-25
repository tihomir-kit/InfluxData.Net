using System;
using System.Collections.Generic;
using System.Linq;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Models.Responses;

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
