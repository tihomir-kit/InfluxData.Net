using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace InfluxData.Net.InfluxDb.RequestClients.Modules
{
    internal class DatabaseRequestModule : RequestModuleBase, IDatabaseRequestModule
    {
        public DatabaseRequestModule(IInfluxDbRequestClient requestClient) 
            : base(requestClient)
        {
        }

        public async Task<IInfluxDbApiResponse> CreateDatabase(string dbName)
        {
            var query = String.Format(QueryStatements.CreateDatabase, dbName);
            return await this.RequestClient.GetQueryAsync(requestParams: RequestClientUtility.BuildQueryRequestParams(query));
        }

        public async Task<IInfluxDbApiResponse> DropDatabase(string dbName)
        {
            var query = String.Format(QueryStatements.DropDatabase, dbName);
            return await this.RequestClient.GetQueryAsync(requestParams: RequestClientUtility.BuildQueryRequestParams(query));
        }

        public async Task<IInfluxDbApiResponse> ShowDatabases()
        {
            return await this.RequestClient.GetQueryAsync(requestParams: RequestClientUtility.BuildQueryRequestParams(QueryStatements.ShowDatabases));
        }

        public async Task<IInfluxDbApiResponse> DropSeries(string dbName, string serieName)
        {
            var query = String.Format(QueryStatements.DropSeries, serieName);
            return await this.RequestClient.GetQueryAsync(requestParams: RequestClientUtility.BuildQueryRequestParams(query));
        }

        public async Task<IInfluxDbApiResponse> AlterRetentionPolicy(string policyName, string dbName, string duration, int replication)
        {
            var requestParams = RequestClientUtility.BuildQueryRequestParams(String.Format(QueryStatements.AlterRetentionPolicy, policyName, dbName, duration, replication));

            return await this.RequestClient.GetQueryAsync(requestParams: requestParams);
        }
    }
}
