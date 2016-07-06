using System;
using InfluxData.Net.Common.Enums;

namespace InfluxData.Net.Common.Infrastructure
{
    public interface IInfluxDbClientConfiguration : IConfiguration
    {
        InfluxDbVersion InfluxVersion { get; }
    }
}