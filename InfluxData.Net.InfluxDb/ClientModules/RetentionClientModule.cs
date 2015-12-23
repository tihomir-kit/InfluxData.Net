using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.RequestClients.Modules;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class RetentionClientModule : ClientModuleBase, IRetentionClientModule
    {
        private readonly IRetentionRequestModule _retentionRequestModule;

        public RetentionClientModule(IInfluxDbRequestClient requestClient, IRetentionRequestModule databaseRequestModule)
            : base(requestClient)
        {
            _retentionRequestModule = databaseRequestModule;
        }

        public async Task<IInfluxDbApiResponse> AlterRetentionPolicy(string dbName, string policyName, string duration, int replicationCopies)
        {
            return await _retentionRequestModule.AlterRetentionPolicy(dbName, policyName, duration, replicationCopies);
        }
    }
}
