using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public interface ICqClientModule
    {
        Task<InfluxDbApiResponse> CreateContinuousQueryAsync(ContinuousQuery continuousQuery);

        /// <summary>
        /// Describe all contious queries in a database.
        /// </summary>
        /// <param name="dbName">The name of the database for which all continuous queries should be described.</param>
        /// <returns>A list of all contious queries.</returns>
        Task<Serie> GetContinuousQueriesAsync(string dbName);

        /// <summary>
        /// Delete a continous query.
        /// </summary>
        /// <param name="dbName">The name of the database for which this query should be deleted.</param>
        /// <param name="cqName">The id of the query.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> DeleteContinuousQueryAsync(string dbName, string cqName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbName"></param>
        /// <param name="backfill"></param>
        /// <returns></returns>
        /// <remarks>PLEASE EXCERSISE CAUTION WITH THIS CALL. Not specifying additional filters might cause your CPU to go to 100% for a long time.</remarks>
        Task<InfluxDbApiResponse> Backfill(string dbName, Backfill backfill);
    }
}