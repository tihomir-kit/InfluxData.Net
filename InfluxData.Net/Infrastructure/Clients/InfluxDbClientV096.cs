using InfluxData.Net.Contracts;
using InfluxData.Net.Enums;
using InfluxData.Net.Infrastructure.Configuration;
using InfluxData.Net.Infrastructure.Formatters;

namespace InfluxData.Net.Infrastructure.Clients
{
    internal class InfluxDbClientV096 : InfluxDbClientV09x
    {
        public InfluxDbClientV096(InfluxDbClientConfiguration configuration) 
            : base(configuration)
        {
        }

        public override IFormatter GetFormatter()
        {
            return new FormatterV096();
        }

        public override InfluxVersion GetVersion()
        {
            return InfluxVersion.v096;
        }
    }
}