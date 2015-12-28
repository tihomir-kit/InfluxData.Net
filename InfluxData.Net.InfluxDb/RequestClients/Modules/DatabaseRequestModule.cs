using System;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Infrastructure;

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

        public async Task<IInfluxDbApiResponse> GetDatabases()
        {
            return await this.RequestClient.GetQueryAsync(requestParams: RequestClientUtility.BuildQueryRequestParams(QueryStatements.GetDatabases));
        }

        public async Task<IInfluxDbApiResponse> DropDatabase(string dbName)
        {
            var query = String.Format(QueryStatements.DropDatabase, dbName);
            return await this.RequestClient.GetQueryAsync(requestParams: RequestClientUtility.BuildQueryRequestParams(query));
        }

        [Obsolete("Plese use 'DropSeries' from .Serie instead.")]
        public async Task<IInfluxDbApiResponse> DropSeries(string dbName, string serieName)
        {
            var query = String.Format(QueryStatements.DropSeries, serieName);
            return await this.RequestClient.GetQueryAsync(requestParams: RequestClientUtility.BuildQueryRequestParams(query));
        }
    }
}
