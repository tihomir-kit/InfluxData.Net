using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.QueryBuilders;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class SerieClientModule : ClientModuleBase, ISerieClientModule
    {
        private readonly ISerieQueryBuilder _serieQueryBuilder;

        public SerieClientModule(IInfluxDbRequestClient requestClient, ISerieQueryBuilder serieQueryBuilder)
            : base(requestClient)
        {
            _serieQueryBuilder = serieQueryBuilder;
        }

        public async Task<IEnumerable<SerieSet>> GetSeriesAsync(string dbName, string measurementName = null, IEnumerable<string> filters = null)
        {
            var query = _serieQueryBuilder.GetSeries(dbName, measurementName, filters);
            var response = await this.GetQueryAsync(dbName, query);
            var queryResult = this.ReadAsQueryResponse(response);

            var result = queryResult.Results.Single();
            var series = GetSeries(result);
            var serieSets = GetSerieSets(series);

            return serieSets;
        }

        private IList<SerieSet> GetSerieSets(IEnumerable<Serie> series)
        {
            var serieSets = new List<SerieSet>();

            foreach (var serie in series)
            {
                var serieSet = GetSerieSet(serie);
                serieSets.Add(serieSet);
            }

            return serieSets;
        }

        private SerieSet GetSerieSet(Serie serie)
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

        private SerieSetItem GetSerieSetItem(int keyIndex, Dictionary<string, int> indexedKeyColumns, object[] serieValues)
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

        public async Task<IInfluxDbApiResponse> DropSeriesAsync(string dbName, string measurementName, IEnumerable<string> filters = null)
        {
            return await DropSeriesAsync(dbName, new List<string>() { measurementName }, filters);
        }

        public async Task<IInfluxDbApiResponse> DropSeriesAsync(string dbName, IEnumerable<string> measurementNames, IEnumerable<string> filters = null)
        {
            var query = _serieQueryBuilder.DropSeries(dbName, measurementNames, filters);
            var response = await this.GetQueryAsync(dbName, query);
            ValidateQueryResponse(response);

            return response;
        }

        public async Task<IEnumerable<Measurement>> GetMeasurementsAsync(string dbName, string withClause = null, IEnumerable<string> filters = null)
        {
            var query = _serieQueryBuilder.GetMeasurements(dbName, withClause, filters);
            var response = await this.GetQueryAsync(dbName, query));
            var queryResult = this.ReadAsQueryResponse(response);

            var measurements = new List<Measurement>();

            var series = queryResult.Results.Single().Series;
            if (series == null)
                return measurements;

            measurements.AddRange(series.Single().Values.Select(p => new Measurement()
            {
                Name = (string)p[0]
            }));

            return measurements;
        }

        public async Task<IInfluxDbApiResponse> DropMeasurementAsync(string dbName, string measurementName)
        {
            var query = _serieQueryBuilder.DropMeasurement(dbName, measurementName);
            var response = await this.GetQueryAsync(dbName, query);
            ValidateQueryResponse(response);

            return response;
        }
    }
}
