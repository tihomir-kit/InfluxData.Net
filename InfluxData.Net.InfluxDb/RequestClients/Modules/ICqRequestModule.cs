using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;

namespace InfluxData.Net.InfluxDb.RequestClients.Modules
{
    public interface ICqRequestModule
    {
        Task<InfluxDbApiResponse> CreateContinuousQuery(ContinuousQuery continuousQuery);

        Task<InfluxDbApiResponse> DeleteContinuousQuery(string dbName, string cqName);

        Task<InfluxDbApiResponse> GetContinuousQueries(string dbName);

        Task<InfluxDbApiResponse> Backfill(string dbName, Backfill backfill);
    }
}