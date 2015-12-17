using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;

namespace InfluxData.Net.InfluxDb.RequestClients.Modules
{
    public interface IDatabaseRequestModule
    {
        Task<IInfluxDbApiResponse> CreateDatabase(string dbName);

        Task<IInfluxDbApiResponse> DropDatabase(string dbName);

        Task<IInfluxDbApiResponse> DropSeries(string dbName, string serieName);

        Task<IInfluxDbApiResponse> ShowDatabases();

        Task<IInfluxDbApiResponse> AlterRetentionPolicy(string policyName, string dbName, string duration, int replication);
    }
}