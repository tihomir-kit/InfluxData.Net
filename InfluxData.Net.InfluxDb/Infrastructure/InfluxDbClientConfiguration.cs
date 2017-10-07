using System;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Infrastructure;
using System.Net.Http;

namespace InfluxData.Net.InfluxDb.Infrastructure
{
    public class InfluxDbClientConfiguration : IInfluxDbClientConfiguration
    {
        public Uri EndpointUri { get; private set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public bool ThrowOnWarning { get; private set; }

        public InfluxDbVersion InfluxVersion { get; private set; }

        public QueryLocation QueryLocation { get; private set; }

        public HttpClient HttpClient { get; private set; }

        /// <summary>
        /// InfluxDb client configuration.
        /// </summary>
        /// <param name="endpointUri">InfluxDb server URI.</param>
        /// <param name="username">InfluxDb server username.</param>
        /// <param name="password">InfluxDb server password.</param>
        /// <param name="influxVersion">InfluxDb server version.</param>
        /// <param name="queryLocation">Where queries are located in the request (URI params vs. Form Data) (optional).</param>
        /// <param name="httpClient">Custom HttpClient object (optional).</param>
        /// <param name="throwOnWarning">Should throw exception upon InfluxDb warning message (for debugging) (optional).</param>
        public InfluxDbClientConfiguration(
            Uri endpointUri, 
            string username, 
            string password, 
            InfluxDbVersion influxVersion,
            QueryLocation queryLocation = QueryLocation.FormData,
            HttpClient httpClient = null, 
            bool throwOnWarning = false)
        {
            Validate.IsNotNull(endpointUri, "Endpoint may not be null or empty.");

            EndpointUri = SanitizeEndpoint(endpointUri, false);
            Username = username;
            Password = password;
            InfluxVersion = influxVersion;
            QueryLocation = queryLocation;
            HttpClient = httpClient ?? new HttpClient();
            ThrowOnWarning = throwOnWarning;
        }

        private static Uri SanitizeEndpoint(Uri endpointUri, bool isTls)
        {
            var builder = new UriBuilder(endpointUri);

            if (isTls)
            {
                builder.Scheme = "https";
            }
            else if (builder.Scheme.Equals("tcp", StringComparison.CurrentCultureIgnoreCase)) // InvariantCultureIgnoreCase, not supported in PCL
            {
                builder.Scheme = "http";
            }

            return builder.Uri;
        }
    }
}