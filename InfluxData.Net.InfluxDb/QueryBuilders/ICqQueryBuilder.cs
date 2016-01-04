using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using System;

namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    public interface ICqQueryBuilder
    {
        /// <summary>
        /// Builds a "create continuous query" query.
        /// </summary>
        /// <param name="continuousQuery">Cq request object which describes the Cq that wants to be created.</param>
        /// <returns></returns>
        string CreateContinuousQuery(ContinuousQuery continuousQuery);

        /// <summary>
        /// Builds a "get all contious queries" query.
        /// </summary>
        /// <returns></returns>
        string GetContinuousQueries();

        /// <summary>
        /// Builds a "delete a continous query" query.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="cqName">The id of the query.</param>
        /// <returns></returns>
        string DeleteContinuousQuery(string dbName, string cqName);

        /// <summary>
        /// Builds a "backfill the database" query.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="backfill">Backfill object which describes the backfill action to execute.</param>
        /// <returns></returns>
        string Backfill(string dbName, Backfill backfill);
    }
}