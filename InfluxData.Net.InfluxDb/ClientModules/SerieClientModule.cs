using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.QueryBuilders;
using InfluxData.Net.InfluxDb.ResponseParsers;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class SerieClientModule : ClientModuleBase, ISerieClientModule
    {
        private readonly ISerieQueryBuilder _serieQueryBuilder;
        private readonly ISerieResponseParser _serieResponseParser;

        public SerieClientModule(IInfluxDbRequestClient requestClient, ISerieQueryBuilder serieQueryBuilder, ISerieResponseParser serieResponseParser)
            : base(requestClient)
        {
            _serieQueryBuilder = serieQueryBuilder;
            _serieResponseParser = serieResponseParser;
        }

        public async Task<IEnumerable<SerieSet>> GetSeriesAsync(string dbName, string measurementName = null, IEnumerable<string> filters = null)
        {
            var query = _serieQueryBuilder.GetSeries(dbName, measurementName, filters);
            var series = await base.ResolveSingleGetSeriesResultAsync(dbName, query);
            var serieSets = _serieResponseParser.GetSerieSets(series);

            return serieSets;
        }

        public async Task<IInfluxDbApiResponse> DropSeriesAsync(string dbName, string measurementName, IEnumerable<string> filters = null)
        {
            return await DropSeriesAsync(dbName, new List<string>() { measurementName }, filters);
        }

        public async Task<IInfluxDbApiResponse> DropSeriesAsync(string dbName, IEnumerable<string> measurementNames, IEnumerable<string> filters = null)
        {
            var query = _serieQueryBuilder.DropSeries(dbName, measurementNames, filters);
            var response = await base.GetAndValidateQueryAsync(dbName, query);

            return response;
        }

        public async Task<IEnumerable<Measurement>> GetMeasurementsAsync(string dbName, string withClause = null, IEnumerable<string> filters = null)
        {
            var query = _serieQueryBuilder.GetMeasurements(dbName, withClause, filters);
            var series = await base.ResolveSingleGetSeriesResultAsync(dbName, query);
            var measurements = _serieResponseParser.GetMeasurements(series);

            return measurements;
        }

        public async Task<IInfluxDbApiResponse> DropMeasurementAsync(string dbName, string measurementName)
        {
            var query = _serieQueryBuilder.DropMeasurement(dbName, measurementName);
            var response = await base.GetAndValidateQueryAsync(dbName, query);

            return response;
        }
    }
}
