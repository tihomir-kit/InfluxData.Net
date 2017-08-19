using InfluxData.Net.Common.Enums;

namespace InfluxData.Net.Common.Infrastructure
{
    public interface IInfluxDbClientConfiguration : IConfiguration
    {
        /// <summary>
        /// InfluxDb server version.
        /// </summary>
        InfluxDbVersion InfluxVersion { get; }
    }
}