using System.Net;

namespace InfluxData.Net.Common.Infrastructure
{
    public class InfluxDataApiResponse : IInfluxDataApiResponse
    {
        public InfluxDataApiResponse(HttpStatusCode statusCode, string body)
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

    public class InfluxDataApiWriteResponse : InfluxDataApiResponse
    {
        public InfluxDataApiWriteResponse(HttpStatusCode statusCode, string body)
             : base(statusCode, body)
        {
        }

        public override bool Success
        {
            get { return StatusCode == HttpStatusCode.NoContent; }
        }
    }

    public class InfluxDataApiDeleteResponse : InfluxDataApiResponse
    {
        public InfluxDataApiDeleteResponse(HttpStatusCode statusCode, string body)
             : base(statusCode, body)
        {
        }

        public override bool Success
        {
            get { return StatusCode == HttpStatusCode.NoContent; }
        }
    }
}