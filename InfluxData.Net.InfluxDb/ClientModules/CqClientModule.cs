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
using InfluxData.Net.InfluxDb.ResponseParsers;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class CqClientModule : ClientModuleBase, ICqClientModule
    {
        private readonly ICqQueryBuilder _cqQueryBuilder;
        private readonly ICqResponseParser _cqResponseParser;

        public CqClientModule(IInfluxDbRequestClient requestClient, ICqQueryBuilder cqQueryBuilder, ICqResponseParser cqResponseParser)
            : base(requestClient)
        {
            _cqQueryBuilder = cqQueryBuilder;
            _cqResponseParser = cqResponseParser;
        }

        public virtual async Task<IInfluxDbApiResponse> CreateContinuousQueryAsync(CqParams cqParams)
        {
            var query = _cqQueryBuilder.CreateContinuousQuery(cqParams);
            var response = await base.GetAndValidateQueryAsync(cqParams.DbName, query);

            return response;
        }

        public virtual async Task<IEnumerable<ContinuousQuery>> GetContinuousQueriesAsync(string dbName)
        {
            var query = _cqQueryBuilder.GetContinuousQueries();
            var series = await base.ResolveSingleGetSeriesResultAsync(dbName, query);
            var cqs = _cqResponseParser.GetContinuousQueries(dbName, series);

            return cqs;
        }

        public virtual async Task<IInfluxDbApiResponse> DeleteContinuousQueryAsync(string dbName, string cqName)
        {
            var query = _cqQueryBuilder.DeleteContinuousQuery(dbName, cqName);
            var response = await base.GetAndValidateQueryAsync(dbName, query);

            return response;
        }

        public virtual async Task<IInfluxDbApiResponse> BackfillAsync(string dbName, BackfillParams backfillParams)
        {
            var query = _cqQueryBuilder.Backfill(dbName, backfillParams);
            var response = await base.GetAndValidateQueryAsync(dbName, query);

            return response;
        }
    }
}
