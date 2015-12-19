using System.Net;

namespace InfluxData.Net.InfluxDb.Infrastructure
{
    public interface IInfluxDbApiResponse
    {
        HttpStatusCode StatusCode { get; }

        string Body { get; }

        bool Success { get; }
    }
}