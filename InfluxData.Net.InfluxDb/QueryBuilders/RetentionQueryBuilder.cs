using System;
using InfluxData.Net.InfluxDb.Constants;

namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    internal class RetentionQueryBuilder : IRetentionQueryBuilder
    {
        public virtual string AlterRetentionPolicy(string dbName, string policyName, string duration, int replication)
        {
            var query = String.Format(QueryStatements.AlterRetentionPolicy, policyName, dbName, duration, replication);

            return query;
        }
    }
}
