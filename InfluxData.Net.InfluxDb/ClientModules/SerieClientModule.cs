using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.QueryBuilders;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.ResponseParsers;
using InfluxData.Net.InfluxDb.ClientSubModules;
using InfluxData.Net.Common.Constants;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class SerieClientModule : ClientModuleBase, ISerieClientModule
    {
        private readonly ISerieQueryBuilder _serieQueryBuilder;
        private readonly ISerieResponseParser _serieResponseParser;
        private readonly IBatchWriter _batchWriter;

        public SerieClientModule(IInfluxDbRequestClient requestClient, ISerieQueryBuilder serieQueryBuilder, ISerieResponseParser serieResponseParser, IBatchWriter batchWriter)
            : base(requestClient)
        {
            _serieQueryBuilder = serieQueryBuilder;
            _serieResponseParser = serieResponseParser;
            _batchWriter = batchWriter;
        }

        public virtual async Task<IEnumerable<SerieSet>> GetSeriesAsync(string dbName, string measurementName = null, IEnumerable<string> filters = null)
        {
            var query = _serieQueryBuilder.GetSeries(dbName, measurementName, filters);
            var series = await base.ResolveSingleGetSeriesResultAsync(query, dbName).ConfigureAwait(false);
            var serieSets = _serieResponseParser.GetSerieSets(series);

            return serieSets;
        }

        public virtual async Task<IInfluxDataApiResponse> DropSeriesAsync(string dbName, string measurementName, IEnumerable<string> filters = null)
        {
            return await DropSeriesAsync(dbName, new List<string>() { measurementName }, filters).ConfigureAwait(false);
        }

        public virtual async Task<IInfluxDataApiResponse> DropSeriesAsync(string dbName, IEnumerable<string> measurementNames, IEnumerable<string> filters = null)
        {
            var query = _serieQueryBuilder.DropSeries(dbName, measurementNames, filters);
            var response = await base.GetAndValidateQueryAsync(query, dbName).ConfigureAwait(false);

            return response;
        }

        public virtual async Task<IEnumerable<Measurement>> GetMeasurementsAsync(string dbName, IEnumerable<string> filters = null)
        {
            var query = _serieQueryBuilder.GetMeasurements(dbName, filters);
            var series = await base.ResolveSingleGetSeriesResultAsync(query, dbName).ConfigureAwait(false);
            var measurements = _serieResponseParser.GetMeasurements(series);

            return measurements;
        }

        public virtual async Task<IInfluxDataApiResponse> DropMeasurementAsync(string dbName, string measurementName)
        {
            var query = _serieQueryBuilder.DropMeasurement(dbName, measurementName);
            var response = await base.GetAndValidateQueryAsync(query, dbName).ConfigureAwait(false);

            return response;
        }

        public virtual async Task<IEnumerable<string>> GetTagKeysAsync(string dbName, string measurementName)
        {
            var query = _serieQueryBuilder.GetTagKeys(dbName, measurementName);
            var series = await base.ResolveSingleGetSeriesResultAsync(query, dbName).ConfigureAwait(false);
            var tagKeys = _serieResponseParser.GetTagKeys(series);

            return tagKeys;
        }

        public virtual async Task<IEnumerable<TagValue>> GetTagValuesAsync(string dbName, string measurementName, string tagName)
        {
            var query = _serieQueryBuilder.GetTagValues(dbName, measurementName, tagName);
            var series = await base.ResolveSingleGetSeriesResultAsync(query, dbName).ConfigureAwait(false);
            var tagValues = _serieResponseParser.GetTagValues(series);

            return tagValues;
        }

        public virtual async Task<IEnumerable<FieldKey>> GetFieldKeysAsync(string dbName, string measurementName)
        {
            var query = _serieQueryBuilder.GetFieldKeys(dbName, measurementName);
            var series = await base.ResolveSingleGetSeriesResultAsync(query, dbName).ConfigureAwait(false);
            var fieldKeys = _serieResponseParser.GetFieldKeys(series);

            return fieldKeys;
        }

        public IBatchWriter CreateBatchWriter(string dbName, string retenionPolicy = null, string precision = TimeUnit.Milliseconds)
        {
            return ((IBatchWriterFactory)_batchWriter).CreateBatchWriter(dbName, retenionPolicy, precision);
        }
    }
}
