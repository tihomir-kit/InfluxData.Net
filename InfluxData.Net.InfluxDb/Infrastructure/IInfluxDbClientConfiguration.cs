using System;
using InfluxData.Net.Common.Enums;

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