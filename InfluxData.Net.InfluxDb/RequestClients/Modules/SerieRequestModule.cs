using System;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Infrastructure;
using System.Collections.Generic;
using InfluxData.Net.Common.Helpers;
using System.Linq;

namespace InfluxData.Net.InfluxDb.RequestClients.Modules
{
    internal class SerieRequestModule : RequestModuleBase, ISerieRequestModule
    {
        public SerieRequestModule(IInfluxDbRequestClient requestClient) 
            : base(requestClient)
        {
        }

        public async Task<IInfluxDbApiResponse> GetSeries(string dbName, string measurementName, IList<string> filters = null)
        {
            var query = QueryStatements.GetSeries;

            if (!String.IsNullOrEmpty(measurementName))
            {
                query = String.Join(" FROM ", query, measurementName);
            }

            if (filters != null && filters.Count > 0)
            {
                query = String.Join(" WHERE ", query, filters.ToCommaSpaceSeparatedString());
            }

            return await this.RequestClient.GetQueryAsync(requestParams: RequestClientUtility.BuildQueryRequestParams(dbName, query));
        }

        public async Task<IInfluxDbApiResponse> DropSeries(string dbName, string measurementName, IList<string> filters = null)
        {
            return await DropSeries(dbName, new List<string>() { measurementName }, filters);
        }

        public async Task<IInfluxDbApiResponse> DropSeries(string dbName, IList<string> measurementNames, IList<string> filters = null)
        {
            var query = String.Format(QueryStatements.DropSeries, measurementNames.ToCommaSeparatedString());

            if (filters != null && filters.Count > 0)
            {
                query = String.Join(" WHERE ", query, filters.ToCommaSpaceSeparatedString());
            }

            return await this.RequestClient.GetQueryAsync(requestParams: RequestClientUtility.BuildQueryRequestParams(dbName, query));
        }

        public async Task<IInfluxDbApiResponse> GetMeasurements(string dbName, string withClause = null, IList<string> filters = null)
        {
            var query = QueryStatements.GetMeasurements;

            if (!String.IsNullOrEmpty(withClause))
            {
                query = String.Join(" WITH MEASUREMENT ", query, withClause);
            }

            if (filters != null && filters.Count > 0)
            {
                query = String.Join(" WHERE ", query, filters.ToCommaSpaceSeparatedString());
            }

            return await this.RequestClient.GetQueryAsync(requestParams: RequestClientUtility.BuildQueryRequestParams(dbName, query));
        }

        public async Task<IInfluxDbApiResponse> DropMeasurement(string dbName, string measurementName)
        {
            var query = String.Format(QueryStatements.DropMeasurement, measurementName);
            return await this.RequestClient.GetQueryAsync(requestParams: RequestClientUtility.BuildQueryRequestParams(dbName, query));
        }
    }
}
