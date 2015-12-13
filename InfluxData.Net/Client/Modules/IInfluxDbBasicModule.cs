using System.Threading.Tasks;
using InfluxData.Net.Infrastructure.Influx;
using InfluxData.Net.Models;

namespace InfluxData.Net.Client.Modules
{
    internal interface IInfluxDbBasicModule
    {
        Task<InfluxDbApiResponse> Query(string dbName, string query);

        Task<InfluxDbApiWriteResponse> Write(WriteRequest writeRequest, string timePrecision);
    }
}