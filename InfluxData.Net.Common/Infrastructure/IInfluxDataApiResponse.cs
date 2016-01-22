using System.Net;

namespace InfluxData.Net.Common.Infrastructure
{
    public interface IInfluxDataApiResponse
    {
        HttpStatusCode StatusCode { get; }

        string Body { get; }

        bool Success { get; }
    }
}