using InfluxData.Net.Contracts;
using InfluxData.Net.Enums;
using InfluxData.Net.Infrastructure.Configuration;
using InfluxData.Net.Infrastructure.Formatters;

namespace InfluxData.Net.Infrastructure.Clients
{
    internal class InfluxDbClientV095 : InfluxDbClientV09x
    {
        public InfluxDbClientV095(InfluxDbClientConfiguration configuration) 
            : base(configuration)
        {
        }

        public override IFormatter GetFormatter()
        {
            return new FormatterV095();
        }

        public override InfluxVersion GetVersion()
        {
            return InfluxVersion.v095;
        }
    }
}