using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using System.Threading.Tasks;

namespace InfluxData.Net.InfluxDb.RequestClients.Modules
{
    public interface IBasicRequestModule
    {
        Task<IInfluxDbApiResponse> Query(string dbName, string query);

        Task<IInfluxDbApiResponse> Write(WriteRequest writeRequest, string timePrecision);
    }
}