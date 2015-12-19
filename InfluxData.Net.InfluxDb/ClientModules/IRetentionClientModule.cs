using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using System.Collections.Generic;

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
        Task<IInfluxDbApiResponse> AlterRetentionPolicy(string dbName, string policyName, string duration, int replicationCopies);
    }
}