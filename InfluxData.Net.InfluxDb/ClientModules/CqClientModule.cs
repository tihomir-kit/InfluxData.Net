using System.Linq;
using System.Net;
using System.Threading.Tasks;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;
using System;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.QueryBuilders;
using System.Collections.Generic;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class CqClientModule : ClientModuleBase, ICqClientModule
    {
        private readonly ICqQueryBuilder _cqQueryBuilder;

        public CqClientModule(IInfluxDbRequestClient requestClient, ICqQueryBuilder cqQueryBuilder)
            : base(requestClient)
        {
            _cqQueryBuilder = cqQueryBuilder;
        }

        public async Task<IInfluxDbApiResponse> CreateContinuousQueryAsync(CqParams cqParams)
        {
            var query = _cqQueryBuilder.CreateContinuousQuery(cqParams);
            var response = await this.GetQueryAsync(cqParams.DbName, query);
            this.ValidateQueryResponse(response);

            return response;
        }

        public async Task<IEnumerable<ContinuousQuery>> GetContinuousQueriesAsync(string dbName)
        {
            var query = _cqQueryBuilder.GetContinuousQueries();
            var response = await this.GetQueryAsync(dbName, query);
            var queryResult = this.ReadAsQueryResponse(response);

            var cqs = new List<ContinuousQuery>();

            var series = queryResult.Results.Single().Series; // TODO: test if InfluxDB ever returns null '.Series'
            if (series == null)
                return cqs;

            var serie = series.FirstOrDefault(p => p.Name == dbName);
            if (serie == null || serie.Values == null)
                return cqs;

            var columns = serie.Columns.ToArray();
            var indexOfName = Array.IndexOf(columns, "name");
            var indexOfQuery = Array.IndexOf(columns, "query");

            cqs.AddRange(serie.Values.Select(p => new ContinuousQuery()
            {
                Name = (string)p[indexOfName],
                Query = (string)p[indexOfQuery]
            }));

            return cqs;
        }

        public async Task<IInfluxDbApiResponse> DeleteContinuousQueryAsync(string dbName, string cqName)
        {
            var query = _cqQueryBuilder.DeleteContinuousQuery(dbName, cqName);
            var response = await this.GetQueryAsync(dbName, query);
            this.ValidateQueryResponse(response);

            return response;
        }

        public async Task<IInfluxDbApiResponse> BackfillAsync(string dbName, BackfillParams backfillParams)
        {
            var query = _cqQueryBuilder.Backfill(dbName, backfillParams);
            var response = await this.GetQueryAsync(dbName, query);
            this.ValidateQueryResponse(response);

            return response;
        }
    }
}
