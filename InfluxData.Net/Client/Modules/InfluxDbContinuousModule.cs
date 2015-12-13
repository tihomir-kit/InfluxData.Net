using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfluxData.Net.Infrastructure.Configuration;
using InfluxData.Net.Constants;
using InfluxData.Net.Models;
using System.Threading.Tasks;
using InfluxData.Net.Infrastructure.Influx;
using InfluxData.Net.Client;
using InfluxData.Net.Enums;

namespace InfluxData.Net.Client.Modules
{
    internal class InfluxDbContinuousModule : InfluxDbModule, IInfluxDbContinuousModule
    {
        public InfluxDbContinuousModule(IInfluxDbClient client) 
            : base(client)
        {
        }

        public async Task<InfluxDbApiResponse> CreateContinuousQuery(CqRequest cqRequest)
        {
            // TODO: perhaps extract subquery and query building to formatter
            var subQuery = String.Format(QueryStatements.CreateContinuousQuerySubQuery, String.Join(",", cqRequest.Downsamplers),
                cqRequest.DsSerieName, cqRequest.SourceSerieName, cqRequest.Interval);

            if (cqRequest.Tags != null)
            {
                var tagsString = String.Join(",", cqRequest.Tags);
                if (!String.IsNullOrEmpty(tagsString))
                    String.Join(", ", subQuery, tagsString);
            }

            if (cqRequest.FillType != FillType.Null)
                String.Join(" ", subQuery, cqRequest.FillType.ToString().ToLower());

            var query = String.Format(QueryStatements.CreateContinuousQuery, cqRequest.CqName, cqRequest.DbName, subQuery);

            return await this.Client.GetQueryAsync(requestParams: InfluxDbClientUtility.BuildQueryRequestParams(cqRequest.DbName, query));
        }

        public async Task<InfluxDbApiResponse> GetContinuousQueries(string dbName)
        {
            return await this.Client.GetQueryAsync(requestParams: InfluxDbClientUtility.BuildQueryRequestParams(dbName, QueryStatements.ShowContinuousQueries));
        }

        public async Task<InfluxDbApiResponse> DeleteContinuousQuery(string dbName, string cqName)
        {
            var query = String.Format(QueryStatements.DropContinuousQuery, cqName, dbName);
            return await this.Client.GetQueryAsync(requestParams: InfluxDbClientUtility.BuildQueryRequestParams(dbName, query));
        }
    }
}
