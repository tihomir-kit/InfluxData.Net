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

        public async Task<IEnumerable<Database>> GetDatabasesAsync()
        {
            var query = _databaseQueryBuilder.GetDatabases();
            var response = await this.GetQueryAsync(query);
            var queryResult = this.ReadAsQueryResponse(response);

            var databases = new List<Database>();

            var series = queryResult.Results.Single().Series;
            if (series == null)
                return databases;

            databases.AddRange(series.Single().Values.Select(p => new Database()
            {
                Name = (string)p[0]
            }));

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
