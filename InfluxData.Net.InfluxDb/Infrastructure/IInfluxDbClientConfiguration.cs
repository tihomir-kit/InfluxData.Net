using System;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Helpers;

namespace InfluxData.Net.InfluxDb.Infrastructure
{
    public interface IInfluxDbClientConfiguration
    {
        Uri EndpointBaseUri { get; }

        string Username { get; }

        string Password { get; }

        InfluxDbVersion InfluxVersion { get; }
    }
}