using System;
using InfluxData.Net.Common.Enums;
using System.Net.Http;

namespace InfluxData.Net.Common.Infrastructure
{
    public interface IConfiguration
    {
        Uri EndpointUri { get; }

        string Username { get; }

        string Password { get; }

        HttpClient HttpClient { get; }

        bool ThrowOnWarning { get; }
    }
}