using System.Collections.Generic;
using System.Linq;
using InfluxData.Net.InfluxDb.Models.Responses;
using System;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    internal class SerieResponseParser : ISerieResponseParser
    {
        protected virtual string KeyColumnName
        {
            get { return "key"; }
        }

        public virtual IEnumerable<SerieSet> GetSerieSets(IEnumerable<Serie> series)
        {
            var serieSets = new List<SerieSet>();

            if (series == null)
                return serieSets;

            foreach (var serie in series)
            {
                var serieSetItems = GetSerieSetItems(serie);

                foreach (var serieSetItem in serieSetItems)
                {
                    BindSerieSets(serieSets, serieSetItem);
                }
            }

            return serieSets;
        }

        protected virtual IList<SerieSetItem> GetSerieSetItems(Serie serie)
        {
            IList<SerieSetItem> series = new List<SerieSetItem>();
            var keyIndex = serie.Columns.IndexOf(KeyColumnName);
            var indexedKeyColumns = Enumerable.Range(0, serie.Columns.Count).ToDictionary(p => serie.Columns[p], p => p);

            foreach (var serieValues in serie.Values)
            {
                var serieSetItem = GetSerieSetItem(keyIndex, indexedKeyColumns, serieValues);
                series.Add(serieSetItem);
            }

            return series;
        }

        protected virtual SerieSetItem GetSerieSetItem(int keyIndex, Dictionary<string, int> indexedKeyColumns, IList<object> serieValues)
        {
            var serieSetItemTags = new Dictionary<string, string>();

            foreach (var tag in indexedKeyColumns)
            {
                if (tag.Key != KeyColumnName)
                    serieSetItemTags.Add(tag.Key, (string)serieValues[tag.Value]);
            }

            var serieSetItem = new SerieSetItem()
            {
                Key = (string)serieValues[keyIndex],
                Tags = serieSetItemTags
            };

            return serieSetItem;
        }

        protected virtual void BindSerieSets(List<SerieSet> serieSets, SerieSetItem serieSetItem)
        {
            var serieKeyValues = serieSetItem.Key.Split(',');
            var serieName = serieKeyValues.FirstOrDefault();

            if (!String.IsNullOrEmpty(serieName) && !serieSets.Any(p => p.Name == serieName))
            {
                var serieSet = new SerieSet() { Name = serieName };
                serieSet.Series.Add(serieSetItem);
                serieSets.Add(serieSet);
            }
            else
            {
                var serieSet = serieSets.FirstOrDefault(p => p.Name == serieName);
                serieSet.Series.Add(serieSetItem);
            }
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

        public virtual IEnumerable<string> GetTagKeys(IEnumerable<Serie> series)
        {
            var tagKeys = new List<string>();

            if (series == null || series.Count() == 0)
                return tagKeys;

            tagKeys.AddRange(series.Single().Values.Select(p => (string)p[0]));

            return tagKeys;
        }

        public virtual IEnumerable<TagValue> GetTagValues(IEnumerable<Serie> series)
        {
            var tagValues = new List<TagValue>();

            if (series == null || series.Count() == 0)
                return tagValues;

            tagValues.AddRange(series.Single().Values.Select(p => new TagValue() { Name = (string)p[0], Value = (string)p[1] }));

            return tagValues;
        }

        public virtual IEnumerable<FieldKey> GetFieldKeys(IEnumerable<Serie> series)
        {
            var fieldKeys = new List<FieldKey>();

            if (series == null || series.Count() == 0)
                return fieldKeys;

            fieldKeys.AddRange(series.Single().Values.Select(p => new FieldKey() { Name = (string)p[0], Type = (string)p[1] }));

            return fieldKeys;
        }
    }
}
