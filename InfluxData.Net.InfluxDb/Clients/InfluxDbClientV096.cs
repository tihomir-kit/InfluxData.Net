using InfluxData.Net.InfluxDb.Formatters;
using InfluxData.Net.InfluxDb.Infrastructure;

namespace InfluxData.Net.InfluxDb.Clients
{
    internal class InfluxDbClientV096 : InfluxDbClientV09x
    {
        public InfluxDbClientV096(InfluxDbClientConfiguration configuration) 
            : base(configuration)
        {
        }

        public override IInfluxDbFormatter GetFormatter()
        {
            return new InfluxDbFormatterV096();
        }
    }
}