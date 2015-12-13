using InfluxData.Net.Enums;
using InfluxData.Net.Infrastructure.Configuration;
using InfluxData.Net.Infrastructure.Formatters;

namespace InfluxData.Net.Client
{
    internal class InfluxDbClientV092 : InfluxDbClientV09x
    {
        public InfluxDbClientV092(InfluxDbClientConfiguration configuration) 
            : base(configuration)
        {
        }

        public override IFormatter GetFormatter()
        {
            return new FormatterV092();
        }
    }
}