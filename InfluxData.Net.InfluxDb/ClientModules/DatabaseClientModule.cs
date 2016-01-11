using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;
using System;
using InfluxData.Net.InfluxDb.QueryBuilders;
using InfluxData.Net.InfluxDb.ResponseParsers;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class DatabaseClientModule : ClientModuleBase, IDatabaseClientModule
    {
        private readonly IDatabaseQueryBuilder _databaseQueryBuilder;
        private readonly IDatabaseResponseParser _databaseResponseParser;

        public DatabaseClientModule(IInfluxDbRequestClient requestClient, IDatabaseQueryBuilder databaseQueryBuilder, IDatabaseResponseParser databaseResponseParser)
            : base(requestClient)
        {
            _databaseQueryBuilder = databaseQueryBuilder;
            _databaseResponseParser = databaseResponseParser;
        }

        public async Task<IInfluxDbApiResponse> CreateDatabaseAsync(string dbName)
        {
            var query = _databaseQueryBuilder.CreateDatabase(dbName);
            var response = await base.GetQueryAsync(query);

            return response;
        }

        public async Task<IEnumerable<Database>> GetDatabasesAsync()
        {
            var query = _databaseQueryBuilder.GetDatabases();
            var response = await base.GetQueryAsync(query);
            var queryResponse = base.ReadAsQueryResponse(response);
            var databases = _databaseResponseParser.GetDatabases(queryResponse);

            return databases;
        }

        public async Task<IInfluxDbApiResponse> DropDatabaseAsync(string dbName)
        {
            var query = _databaseQueryBuilder.DropDatabase(dbName);
            var response = await base.GetQueryAsync(query);

            return response;
        }
    }
}
