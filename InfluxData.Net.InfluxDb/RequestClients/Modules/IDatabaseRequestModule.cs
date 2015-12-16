using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;

namespace InfluxData.Net.InfluxDb.RequestClients.Modules
{
    internal interface IDatabaseRequestModule
    {
        Task<InfluxDbApiResponse> CreateDatabase(string dbName);

        Task<InfluxDbApiResponse> DropDatabase(string dbName);

        Task<InfluxDbApiResponse> DropSeries(string dbName, string serieName);

        Task<InfluxDbApiResponse> ShowDatabases();

        Task<InfluxDbApiResponse> AlterRetentionPolicy(string policyName, string dbName, string duration, int replication);
    }
}