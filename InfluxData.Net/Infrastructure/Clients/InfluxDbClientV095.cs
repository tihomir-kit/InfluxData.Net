using InfluxData.Net.Infrastructure.Clients;
using InfluxData.Net.Infrastructure.Configuration;
using InfluxData.Net.Infrastructure.Formatters;

namespace InfluxData.Net.Infrastructure.Infrastructure.Clients
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
    }
}