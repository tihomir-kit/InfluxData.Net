using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public interface IRetentionClientModule
    {
        /// <summary>
        /// Alters a retention policy.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="policyName">Retention policy name.</param>
        /// <param name="duration">New data keep duration.</param>
        /// <param name="replicationCopies">Number of independent copies of data in the cluster (number of data nodes).</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> AlterRetentionPolicyAsync(string dbName, string policyName, string duration, int replicationCopies);
    }
}