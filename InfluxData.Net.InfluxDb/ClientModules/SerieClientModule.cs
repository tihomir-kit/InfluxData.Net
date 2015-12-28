using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.RequestClients.Modules;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class SerieClientModule : ClientModuleBase, ISerieClientModule
    {
        private readonly ISerieRequestModule _serieRequestModule;

        public SerieClientModule(IInfluxDbRequestClient requestClient, ISerieRequestModule serieRequestModule)
            : base(requestClient)
        {
            _serieRequestModule = serieRequestModule;
        }

        public async Task<IInfluxDbApiResponse> GetSeriesAsync(string dbName, string measurementName, IList<string> filters = null)
        {
            return await _serieRequestModule.GetSeries(dbName, measurementName, filters);
        }

        public async Task<IInfluxDbApiResponse> DropSeriesAsync(string dbName, string measurementName, IList<string> filters = null)
        {
            return await _serieRequestModule.DropSeries(dbName, measurementName, filters);
        }

        public async Task<IInfluxDbApiResponse> DropSeriesAsync(string dbName, IList<string> measurementNames, IList<string> filters = null)
        {
            return await _serieRequestModule.DropSeries(dbName, measurementNames, filters);
        }

        public async Task<IInfluxDbApiResponse> GetMeasurementsAsync(string dbName, string withClause = null, IList<string> filters = null)
        {
            return await _serieRequestModule.GetMeasurements(dbName, withClause, filters);
        }

        public async Task<IInfluxDbApiResponse> DropMeasurementAsync(string dbName, string measurementName)
        {
            return await _serieRequestModule.DropMeasurement(dbName, measurementName);
        }
    }
}
