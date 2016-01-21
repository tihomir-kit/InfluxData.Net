using System.Threading.Tasks;
using InfluxData.Net.Kapacitor.Infrastructure;
using System;

namespace InfluxData.Net.Kapacitor.QueryBuilders
{
    public interface IDatabaseQueryBuilder
    {
        /// <summary>
        /// Builds "create a new database" query.
        /// </summary>
        /// <param name="dbName">The name of the new database</param>
        /// <returns></returns>
        string CreateDatabase(string dbName);

        /// <summary>
        /// Builds "get all available databases" query.
        /// </summary>
        /// <returns></returns>
        string GetDatabases();

        /// <summary>
        /// Builds "drop a database" query.
        /// </summary>
        /// <param name="dbName">The name of the database to delete.</param>
        /// <returns></returns>
        string DropDatabase(string dbName);
    }
}