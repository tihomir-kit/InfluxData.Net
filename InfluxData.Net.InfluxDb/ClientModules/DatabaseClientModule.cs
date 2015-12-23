using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.RequestClients.Modules;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class DatabaseClientModule : ClientModuleBase, IDatabaseClientModule
    {
        private readonly IDatabaseRequestModule _databaseRequestModule;

        public DatabaseClientModule(IInfluxDbRequestClient requestClient, IDatabaseRequestModule databaseRequestModule)
            : base(requestClient)
        {
            _databaseRequestModule = databaseRequestModule;
        }

        public async Task<IInfluxDbApiResponse> CreateDatabaseAsync(string dbName)
        {
            return await _databaseRequestModule.CreateDatabase(dbName);
        }

        public async Task<IList<DatabaseResponse>> GetDatabasesAsync()
        {
            var response = await _databaseRequestModule.GetDatabases();
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
            return await _databaseRequestModule.DropDatabase(dbName);
        }

        public async Task<IInfluxDbApiResponse> DropSeriesAsync(string dbName, string serieName)
        {
            return await _databaseRequestModule.DropSeries(dbName, serieName);
        }
    }
}
