using System;
using InfluxData.Net.Common.Enums;

namespace InfluxData.Net.Kapacitor.Infrastructure
{
    public interface IKapacitorClientConfiguration
    {
        Uri EndpointUri { get; }

        string Username { get; }

        string Password { get; }

        InfluxDbVersion InfluxVersion { get; }
    }
}