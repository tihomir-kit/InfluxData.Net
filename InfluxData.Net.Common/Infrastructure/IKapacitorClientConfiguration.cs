using InfluxData.Net.Common.Enums;

namespace InfluxData.Net.Common.Infrastructure
{
    public interface IKapacitorClientConfiguration : IConfiguration
    {
        KapacitorVersion KapacitorVersion { get; }
    }
}