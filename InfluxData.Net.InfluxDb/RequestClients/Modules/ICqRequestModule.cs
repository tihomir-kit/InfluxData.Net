using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;

namespace InfluxData.Net.InfluxDb.RequestClients.Modules
{
    public interface ICqRequestModule
    {
        /// <summary>
        /// Creates a continuous query in the database.
        /// </summary>
        /// <param name="continuousQuery">Cq request object which describes the Cq that wants to be created.</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> CreateContinuousQuery(ContinuousQuery continuousQuery);

        /// <summary>
        /// Gets all contious queries from the database.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <returns>A list of all contious queries formatted as a <see cref="{Serie}"/>.</returns>
        Task<IInfluxDbApiResponse> GetContinuousQueries(string dbName);

        /// <summary>
        /// Deletes a continous query.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="cqName">The id of the query.</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> DeleteContinuousQuery(string dbName, string cqName);

        /// <summary>
        /// Backfills the database based on the <see cref="{Backfill}"/> configuration object.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="backfill"></param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> Backfill(string dbName, Backfill backfill);
    }
}