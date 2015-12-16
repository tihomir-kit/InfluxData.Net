using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using System.Threading.Tasks;

namespace InfluxData.Net.InfluxDb.RequestClients.Modules
{
    public interface IBasicRequestModule
    {
        Task<InfluxDbApiResponse> Query(string dbName, string query);

        Task<InfluxDbApiWriteResponse> Write(WriteRequest writeRequest, string timePrecision);
    }
}