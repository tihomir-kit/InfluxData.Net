using System;
using System.Net;

namespace InfluxData.Net.Kapacitor.Infrastructure
{
    public class KapacitorException : Exception
    {
        public KapacitorException(string message, Exception innerException)
             : base(message, innerException)
        {
        }

        public KapacitorException(string message)
             : base(message)
        {
        }
    }

    public class KapacitorApiException : KapacitorException
    {
        public KapacitorApiException(HttpStatusCode statusCode, string responseBody)
             : base(String.Format("Kapacitor API responded with status code={0}, response={1}", statusCode, responseBody))
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }

        public HttpStatusCode StatusCode { get; private set; }

        public string ResponseBody { get; private set; }
    }
}