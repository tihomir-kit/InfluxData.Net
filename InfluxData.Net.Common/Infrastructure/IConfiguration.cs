using System;
using System.Net.Http;

namespace InfluxData.Net.Common.Infrastructure
{
    public interface IConfiguration
    {
        /// <summary>
        /// InfluxDb server URI.
        /// </summary>
        Uri EndpointUri { get; }

        /// <summary>
        /// InfluxDb server username.
        /// </summary>
        string Username { get; }

        /// <summary>
        /// InfluxDb server password.
        /// </summary>
        string Password { get; }

        /// <summary>
        /// Custom HttpClient object (optional).
        /// </summary>
        HttpClient HttpClient { get; }

        /// <summary>
        /// Should throw exception upon InfluxDb warning message (for debugging).
        /// </summary>
        bool ThrowOnWarning { get; }
    }
}