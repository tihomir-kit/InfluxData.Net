using System;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Helpers;

namespace InfluxData.Net.InfluxDb.Infrastructure
{
    public class InfluxDbClientConfiguration : IInfluxDbClientConfiguration
    {
        public Uri EndpointBaseUri { get; internal set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public InfluxDbVersion InfluxVersion { get; private set; }

        public InfluxDbClientConfiguration(Uri endpoint, string username, string password, InfluxDbVersion influxVersion)
        {
            Validate.NotNull(endpoint, "Endpoint may not be null or empty.");
            Validate.NotNullOrEmpty(password, "Password may not be null or empty.");
            Validate.NotNullOrEmpty(username, "Username may not be null or empty.");

            Username = username;
            Password = password;
            InfluxVersion = influxVersion;
            EndpointBaseUri = SanitizeEndpoint(endpoint, false);
        }

        private static Uri SanitizeEndpoint(Uri endpoint, bool isTls)
        {
            var builder = new UriBuilder(endpoint);

            if (isTls)
            {
                builder.Scheme = "https";
            }
            else if (builder.Scheme.Equals("tcp", StringComparison.CurrentCultureIgnoreCase))
            //InvariantCultureIgnoreCase, not supported in PCL
            {
                builder.Scheme = "http";
            }

            return builder.Uri;
        }
    }
}