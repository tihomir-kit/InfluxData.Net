using InfluxData.Net.InfluxDb.Formatters;
using InfluxData.Net.InfluxDb.Infrastructure;

namespace InfluxData.Net.InfluxDb.Clients
{
    internal class InfluxDbClientV092 : InfluxDbClientV09x
    {
        public InfluxDbClientV092(InfluxDbClientConfiguration configuration) 
            : base(configuration)
        {
        }

        public override IInfluxDbFormatter GetFormatter()
        {
            return new InfluxDbFormatterV092();
        }
    }
}