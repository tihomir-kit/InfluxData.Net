using InfluxData.Net.InfluxDb.Helpers;
using InfluxData.Net.InfluxDb.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    public class SerieResponseParser : ISerieResponseParser
    {
        public virtual IEnumerable<SerieSet> GetSerieSets(IEnumerable<Serie> series)
        {
            var serieSets = new List<SerieSet>();

            foreach (var serie in series)
            {
                var serieSet = GetSerieSet(serie);
                serieSets.Add(serieSet);
            }

            return serieSets;
        }

        protected virtual SerieSet GetSerieSet(Serie serie)
        {
            var serieSet = new SerieSet() { Name = serie.Name };
            var keyIndex = serie.Columns.IndexOf("_key");
            var indexedKeyColumns = Enumerable.Range(0, serie.Columns.Count).ToDictionary(p => serie.Columns[p], p => p);

            foreach (var serieValues in serie.Values)
            {
                var serieSetItem = GetSerieSetItem(keyIndex, indexedKeyColumns, serieValues);
                serieSet.Series.Add(serieSetItem);
            }

            return serieSet;
        }

        protected virtual SerieSetItem GetSerieSetItem(int keyIndex, Dictionary<string, int> indexedKeyColumns, object[] serieValues)
        {
            var serieSetItemTags = new Dictionary<string, string>();

            foreach (var tag in indexedKeyColumns)
            {
                if (tag.Key != "_key")
                    serieSetItemTags.Add(tag.Key, (string)serieValues[tag.Value]);
            }

            var serieSetItem = new SerieSetItem()
            {
                Key = (string)serieValues[keyIndex],
                Tags = serieSetItemTags
            };

            return serieSetItem;
        }

        public virtual IEnumerable<Measurement> GetMeasurements(QueryResponse queryResponse)
        {
            var measurements = new List<Measurement>();

            var series = queryResponse.Results.Single().Series;
            if (series == null)
                return measurements;

            measurements.AddRange(series.Single().Values.Select(p => new Measurement()
            {
                Name = (string)p[0]
            }));

            return measurements;
        }
    }
}
