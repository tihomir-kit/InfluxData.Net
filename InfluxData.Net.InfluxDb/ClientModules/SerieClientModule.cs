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

        public async Task<IInfluxDbApiResponse> GetSeriesAsync(string dbName, string measurementName, IEnumerable<string> filters = null)
        {
            var query = _serieQueryBuilder.GetSeries(dbName, measurementName, filters);
            var response = await this.GetQueryAsync(dbName, query);

            // TODO: format to a strongly-typed object

            return response;
        }

        public async Task<IInfluxDbApiResponse> DropSeriesAsync(string dbName, string measurementName, IEnumerable<string> filters = null)
        {
            return await DropSeriesAsync(dbName, new List<string>() { measurementName }, filters);
        }

        public async Task<IInfluxDbApiResponse> DropSeriesAsync(string dbName, IEnumerable<string> measurementNames, IEnumerable<string> filters = null)
        {
            var query = _serieQueryBuilder.DropSeries(dbName, measurementNames, filters);
            var response = await this.GetQueryAsync(dbName, query);

            return response;
        }

        public async Task<IInfluxDbApiResponse> GetMeasurementsAsync(string dbName, string withClause = null, IEnumerable<string> filters = null)
        {
            var query = _serieQueryBuilder.GetMeasurements(dbName, withClause, filters);
            var response = await this.GetQueryAsync(dbName, query);

            // TODO: format to a strongly-typed object

            return response;
        }

        public async Task<IInfluxDbApiResponse> DropMeasurementAsync(string dbName, string measurementName)
        {
            var query = _serieQueryBuilder.DropMeasurement(dbName, measurementName);
            var response = await this.GetQueryAsync(dbName, query);

            return response;
        }
    }
}
