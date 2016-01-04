using System;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Infrastructure;

namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    internal class DatabaseQueryBuilder : IDatabaseQueryBuilder
    {
        public string CreateDatabase(string dbName)
        {
            var query = String.Format(QueryStatements.CreateDatabase, dbName);

            return query;
        }

        public string GetDatabases()
        {
            return QueryStatements.GetDatabases;
        }

        public string DropDatabase(string dbName)
        {
            var query = String.Format(QueryStatements.DropDatabase, dbName);

            return query;
        }
    }
}
