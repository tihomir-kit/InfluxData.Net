using System.Net;

namespace InfluxData.Net.Kapacitor.Infrastructure
{
    public class KapacitorApiResponse : IKapacitorApiResponse
    {
        public KapacitorApiResponse(HttpStatusCode statusCode, string body)
        {
            StatusCode = statusCode;
            Body = body;
        }

        public HttpStatusCode StatusCode { get; private set; }

        public string Body { get; private set; }

        public virtual bool Success
        {
            get { return StatusCode == HttpStatusCode.OK; }
        }
    }

    public class KapacitorApiWriteResponse : KapacitorApiResponse
    {
        public KapacitorApiWriteResponse(HttpStatusCode statusCode, string body)
             : base(statusCode, body)
        {
        }

        public override bool Success
        {
            get { return StatusCode == HttpStatusCode.NoContent; }
        }
    }

    public class KapacitorApiDeleteResponse : KapacitorApiResponse
    {
        public KapacitorApiDeleteResponse(HttpStatusCode statusCode, string body)
             : base(statusCode, body)
        {
        }

        public override bool Success
        {
            get { return StatusCode == HttpStatusCode.NoContent; }
        }
    }
}