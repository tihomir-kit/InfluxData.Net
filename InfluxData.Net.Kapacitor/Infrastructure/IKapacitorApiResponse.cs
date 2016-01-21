using System.Net;

namespace InfluxData.Net.Kapacitor.Infrastructure
{
    public interface IKapacitorApiResponse
    {
        HttpStatusCode StatusCode { get; }

        string Body { get; }

        bool Success { get; }
    }
}