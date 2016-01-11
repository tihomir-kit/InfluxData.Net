using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.QueryBuilders;
using InfluxData.Net.InfluxDb.ResponseParsers;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class RetentionClientModule : ClientModuleBase, IRetentionClientModule
    {
        private readonly IRetentionQueryBuilder _retentionQueryBuilder;
        private readonly IRetentionResponseParser _retentionResponseParser;

        public RetentionClientModule(IInfluxDbRequestClient requestClient, IRetentionQueryBuilder retentionQueryBuilder, IRetentionResponseParser retentionResponseParser)
            : base(requestClient)
        {
            _retentionQueryBuilder = retentionQueryBuilder;
            _retentionResponseParser = retentionResponseParser;
        }

        public async Task<IInfluxDbApiResponse> AlterRetentionPolicyAsync(string dbName, string policyName, string duration, int replicationCopies)
        {
            var query = _retentionQueryBuilder.AlterRetentionPolicy(dbName, policyName, duration, replicationCopies);
            var response = await this.GetQueryAsync(query);

            return response;
        }
    }
}
