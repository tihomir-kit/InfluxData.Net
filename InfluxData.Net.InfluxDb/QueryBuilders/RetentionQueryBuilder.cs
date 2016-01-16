using System;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Infrastructure;

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
