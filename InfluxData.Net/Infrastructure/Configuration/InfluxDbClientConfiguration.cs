using System;
using System.Net.Http;
using InfluxData.Net.Infrastructure;
using InfluxData.Net.Infrastructure.Validation;
using InfluxData.Net.Enums;

namespace InfluxData.Net.Infrastructure.Configuration
{
    public class InfluxDbClientConfiguration
    {
        public Uri EndpointBaseUri { get; internal set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public InfluxVersion InfluxVersion { get; private set; }

        public InfluxDbClientConfiguration(Uri endpoint, string username, string password, InfluxVersion influxVersion)
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