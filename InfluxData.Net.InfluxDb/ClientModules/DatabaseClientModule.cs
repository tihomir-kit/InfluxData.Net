using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;
using System;
using InfluxData.Net.InfluxDb.QueryBuilders;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class DatabaseClientModule : ClientModuleBase, IDatabaseClientModule
    {
        private readonly IDatabaseQueryBuilder _databaseQueryBuilder;

        public DatabaseClientModule(IInfluxDbRequestClient requestClient, IDatabaseQueryBuilder databaseQueryBuilder)
            : base(requestClient)
        {
            _databaseQueryBuilder = databaseQueryBuilder;
        }

        public async Task<IInfluxDbApiResponse> CreateDatabaseAsync(string dbName)
        {
            var query = _databaseQueryBuilder.CreateDatabase(dbName);
            var response = await this.GetQueryAsync(query);

            return response;
        }

        public async Task<IList<DatabaseResponse>> GetDatabasesAsync()
        {
            var query = _databaseQueryBuilder.GetDatabases();
            var response = await this.GetQueryAsync(query);

            var queryResult = response.ReadAs<QueryResponse>();
            var serie = queryResult.Results.Single().Series.Single();
            var databases = new List<DatabaseResponse>();

            foreach (var value in serie.Values)
            {
                databases.Add(new DatabaseResponse
                {
                    Name = (string)value[0]
                });
            }

            return databases;
        }

        public async Task<IInfluxDbApiResponse> DropDatabaseAsync(string dbName)
        {
            var query = _databaseQueryBuilder.DropDatabase(dbName);
            var response = await this.GetQueryAsync(query);

            return response;
        }
    }
}
