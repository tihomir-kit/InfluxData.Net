using System.Threading.Tasks;
using InfluxData.Net.Infrastructure.Influx;
using InfluxData.Net.Models;

namespace InfluxData.Net.Client.Modules
{
    internal interface IInfluxDbContinuousModule
    {
        Task<InfluxDbApiResponse> CreateContinuousQuery(CqRequest cqRequest);

        Task<InfluxDbApiResponse> DeleteContinuousQuery(string dbName, string cqName);

        Task<InfluxDbApiResponse> GetContinuousQueries(string dbName);
    }
}