using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public interface ICqClientModule
    {
        /// <summary>
        /// Creates a continuous query in a database.
        /// </summary>
        /// <param name="continuousQuery">Cq request object which describes the Cq that wants to be created.</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> CreateContinuousQueryAsync(ContinuousQuery continuousQuery);

        /// <summary>
        /// Gets all contious queries in a database.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <returns>A list of all contious queries formatted as a <see cref="{Serie}"/>.</returns>
        Task<Serie> GetContinuousQueriesAsync(string dbName);

        /// <summary>
        /// Deletes a continous query.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="cqName">The id of the query.</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> DeleteContinuousQueryAsync(string dbName, string cqName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbName">Database name</param>
        /// <param name="backfill"></param>
        /// <returns></returns>
        /// <remarks>PLEASE EXCERSISE CAUTION WITH THIS CALL. Not specifying additional filters might cause your CPU to go to 100% for a long time.</remarks>
        Task<IInfluxDbApiResponse> Backfill(string dbName, Backfill backfill);
    }
}