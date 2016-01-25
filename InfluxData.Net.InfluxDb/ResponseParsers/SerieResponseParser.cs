using System.Collections.Generic;
using System.Linq;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    internal class SerieResponseParser : ISerieResponseParser
    {
        public virtual IEnumerable<SerieSet> GetSerieSets(IEnumerable<Serie> series)
        {
            var serieSets = new List<SerieSet>();

            if (series == null)
                return serieSets;

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

        protected virtual SerieSetItem GetSerieSetItem(int keyIndex, Dictionary<string, int> indexedKeyColumns, IList<object> serieValues)
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

        public virtual IEnumerable<Measurement> GetMeasurements(IEnumerable<Serie> series)
        {
            var measurements = new List<Measurement>();

            if (series == null || series.Count() == 0)
                return measurements;

            measurements.AddRange(series.Single().Values.Select(p => new Measurement()
            {
                Name = (string)p[0]
            }));

            return measurements;
        }
    }
}
