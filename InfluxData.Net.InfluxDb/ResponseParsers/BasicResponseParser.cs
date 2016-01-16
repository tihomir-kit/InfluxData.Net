using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Helpers;
using InfluxData.Net.InfluxDb.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    internal class BasicResponseParser : IBasicResponseParser
    {
        public virtual IEnumerable<Serie> FlattenResultsSeries(IEnumerable<SeriesResult> seriesResults)
        {
            var series = new List<Serie>();

            foreach (var result in seriesResults)
            {
                series.AddRange(result.Series ?? new List<Serie>());
            }

            return series;
        }

        public virtual IEnumerable<IEnumerable<Serie>> MapResultsSeries(IEnumerable<SeriesResult> seriesResults)
        {
            return seriesResults.Select(GetSeries);
        }

        protected virtual IEnumerable<Serie> GetSeries(SeriesResult result)
        {
            Validate.IsNotNull(result, "result");
            return result.Series != null ? result.Series.ToList() : new List<Serie>();
        }
    }
}
