using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.Helpers
{
    public static class SerieExtensions
    {
        public static T FirstRecordValueAs<T>(this Serie serie, string columnKey)
        {
            var firstValue = serie.Values.FirstOrDefault();
            if (firstValue == null)
                return default(T);

            return (T)(firstValue[serie.Columns.IndexOf(columnKey)]);
        }

        public static Serie GetByName(this IEnumerable<Serie> series, string serieName)
        {
            var serie = series.FirstOrDefault(p => p.Name == serieName);
            Validate.NotNull(serie, String.Format("serie.GetByName('{0}')", serieName));
            return serie;
        }
    }
}
