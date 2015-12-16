using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InfluxData.Net.InfluxDb.Clients.Modules
{
    internal class InfluxDbDatabaseModule : InfluxDbModule, IInfluxDbDatabaseModule
    {
        public InfluxDbDatabaseModule(IInfluxDbClient client) 
            : base(client)
        {
        }

        public async Task<InfluxDbApiResponse> CreateDatabase(string dbName)
        {
            var query = String.Format(QueryStatements.CreateDatabase, dbName);
            return await this.Client.GetQueryAsync(requestParams: InfluxDbClientUtility.BuildQueryRequestParams(query));
        }

        public async Task<InfluxDbApiResponse> DropDatabase(string dbName)
        {
            var query = String.Format(QueryStatements.DropDatabase, dbName);
            return await this.Client.GetQueryAsync(requestParams: InfluxDbClientUtility.BuildQueryRequestParams(query));
        }

        public async Task<InfluxDbApiResponse> ShowDatabases()
        {
            return await this.Client.GetQueryAsync(requestParams: InfluxDbClientUtility.BuildQueryRequestParams(QueryStatements.ShowDatabases));
        }

        public async Task<InfluxDbApiResponse> DropSeries(string dbName, string serieName)
        {
            var query = String.Format(QueryStatements.DropSeries, serieName);
            return await this.Client.GetQueryAsync(requestParams: InfluxDbClientUtility.BuildQueryRequestParams(query));
        }

        public async Task<InfluxDbApiResponse> AlterRetentionPolicy(string policyName, string dbName, string duration, int replication)
        {
            var requestParams = InfluxDbClientUtility.BuildQueryRequestParams(String.Format(QueryStatements.AlterRetentionPolicy, policyName, dbName, duration, replication));

            return await this.Client.GetQueryAsync(requestParams: requestParams);
        }
    }
}
