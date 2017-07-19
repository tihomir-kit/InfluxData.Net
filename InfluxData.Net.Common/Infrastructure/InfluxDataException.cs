using System;
using System.Net;

namespace InfluxData.Net.Common.Infrastructure
{
    public class InfluxDataException : Exception
    {
        public InfluxDataException(string message, Exception innerException)
             : base(message, innerException)
        {
        }

        public InfluxDataException(string message)
             : base(message)
        {
        }
    }

    public class InfluxDataApiException : InfluxDataException
    {
        public InfluxDataApiException(HttpStatusCode statusCode, string responseBody)
             : base(String.Format("InfluxData API responded with status code={0}, response={1}", statusCode, responseBody))
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }

        public HttpStatusCode StatusCode { get; private set; }

        public string ResponseBody { get; private set; }
    }

    public class InfluxDataWarningException : InfluxDataException
    {
        public InfluxDataWarningException(string warningMessage)
             : base(String.Format("InfluxData API responded with a warning {0}", warningMessage))
        {
            WarningMessage = warningMessage;
        }

        public string WarningMessage { get; private set; }
    }
}