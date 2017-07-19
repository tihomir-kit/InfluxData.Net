using System;
using InfluxData.Net.InfluxDb.Constants;

namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    internal class RetentionQueryBuilder : IRetentionQueryBuilder
    {
        public string CreateRetentionPolicy(string dbName, string policyName, string duration, int replication)
        {
            var query = String.Format(QueryStatements.CreateRetentionPolicy, policyName, dbName, duration, replication);

            return query;
        }

        public virtual string GetRetentionPolicies(string dbName)
        {
            var query = String.Format(QueryStatements.GetRetentionPolicies, dbName);

            return query;
        }

        public virtual string AlterRetentionPolicy(string dbName, string policyName, string duration, int replication)
        {
            var query = String.Format(QueryStatements.AlterRetentionPolicy, policyName, dbName, duration, replication);

            return query;
        }

        public string DropRetentionPolicy(string dbName, string policyName)
        {
            var query = String.Format(QueryStatements.DropRetentionPolicy, policyName, dbName);

            return query;
        }
    }
}
