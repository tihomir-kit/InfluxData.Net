using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;

namespace InfluxData.Net.InfluxDb.RequestClients.Modules
{
    public interface ICqRequestModule
    {
        Task<IInfluxDbApiResponse> CreateContinuousQuery(ContinuousQuery continuousQuery);

        Task<IInfluxDbApiResponse> DeleteContinuousQuery(string dbName, string cqName);

        Task<IInfluxDbApiResponse> GetContinuousQueries(string dbName);

        Task<IInfluxDbApiResponse> Backfill(string dbName, Backfill backfill);
    }
}