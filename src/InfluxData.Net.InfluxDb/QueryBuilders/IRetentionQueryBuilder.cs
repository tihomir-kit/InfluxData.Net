namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    public interface IRetentionQueryBuilder
    {
        /// <summary>
        /// Builds "create retention policy" query.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="policyName">Retention policy name.</param>
        /// <param name="duration">New data keep duration.</param>
        /// <param name="replication">Number of independent copies of data in the cluster (number of data nodes).</param>
        /// <returns></returns>
        string CreateRetentionPolicy(string dbName, string policyName, string duration, int replication);

        /// <summary>
        /// Builds "get retention policies" query.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <returns></returns>
        string GetRetentionPolicies(string dbName);

        /// <summary>
        /// Builds "alter retention policy" query.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="policyName">Retention policy name.</param>
        /// <param name="duration">New data keep duration.</param>
        /// <param name="replication">Number of independent copies of data in the cluster (number of data nodes).</param>
        /// <returns></returns>
        string AlterRetentionPolicy(string dbName, string policyName, string duration, int replication);

        /// <summary>
        /// Builds "drop retention policy" query.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="policyName">Retention policy name.</param>
        /// <returns></returns>
        string DropRetentionPolicy(string dbName, string policyName);
    }
}