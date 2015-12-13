using System.Threading.Tasks;
using InfluxData.Net.Infrastructure.Influx;
using InfluxData.Net.Models;

namespace InfluxData.Net.Infrastructure.Clients.Modules
{
    internal interface IInfluxDbContinuousModule
    {
        Task<InfluxDbApiResponse> CreateContinuousQuery(ContinuousQuery continuousQuery);

        Task<InfluxDbApiResponse> DeleteContinuousQuery(string dbName, string cqName);

        Task<InfluxDbApiResponse> GetContinuousQueries(string dbName);
    }
}