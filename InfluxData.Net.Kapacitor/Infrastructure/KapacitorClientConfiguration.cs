using System;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Helpers;

namespace InfluxData.Net.Kapacitor.Infrastructure
{
    public class KapacitorClientConfiguration : IKapacitorClientConfiguration
    {
        public Uri EndpointUri { get; internal set; }

        public string Username { get; private set; }

        public string Password { get; private set; }

        public KapacitorVersion KapacitorVersion { get; private set; }

        public KapacitorClientConfiguration(Uri endpointUri, string username, string password, KapacitorVersion kapacitorVersion)
        {
            Validate.IsNotNull(endpointUri, "Endpoint may not be null or empty.");
            //Validate.IsNotNullOrEmpty(password, "Password may not be null or empty.");
            //Validate.IsNotNullOrEmpty(username, "Username may not be null or empty.");

            EndpointUri = SanitizeEndpoint(endpointUri, false);
            Username = username;
            Password = password;
            KapacitorVersion = kapacitorVersion;
        }

        private static Uri SanitizeEndpoint(Uri endpointUri, bool isTls)
        {
            var builder = new UriBuilder(endpointUri);

            if (isTls)
            {
                builder.Scheme = "https";
            }
            else if (builder.Scheme.Equals("tcp", StringComparison.CurrentCultureIgnoreCase)) //InvariantCultureIgnoreCase, not supported in PCL
            {
                builder.Scheme = "http";
            }

            return builder.Uri;
        }
    }
}