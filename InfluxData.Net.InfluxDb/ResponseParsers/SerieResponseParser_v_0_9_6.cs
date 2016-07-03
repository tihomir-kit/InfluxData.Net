using System.Collections.Generic;
using System.Linq;
using InfluxData.Net.InfluxDb.Models.Responses;
using System;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    internal class SerieResponseParser_v_0_9_6 : SerieResponseParser
    {
        protected override string KeyColumnName
        {
            get { return "_key"; }
        }

        public override IEnumerable<SerieSet> GetSerieSets(IEnumerable<Serie> series)
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
            var keyIndex = serie.Columns.IndexOf(KeyColumnName);
            var indexedKeyColumns = Enumerable.Range(0, serie.Columns.Count).ToDictionary(p => serie.Columns[p], p => p);

            foreach (var serieValues in serie.Values)
            {
                var serieSetItem = GetSerieSetItem(keyIndex, indexedKeyColumns, serieValues);
                serieSet.Series.Add(serieSetItem);
            }

            return serieSet;
        }

        protected override void BindSerieSets(List<SerieSet> serieSets, SerieSetItem serieSetItem)
        {
            throw new InvalidOperationException("Method not applicable to this version of InfluxDB");
        }

        protected override IList<SerieSetItem> GetSerieSetItems(Serie serie)
        {
            throw new InvalidOperationException("Method not applicable to this version of InfluxDB");
        }
    }
}
