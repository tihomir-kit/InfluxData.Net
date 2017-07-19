using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Formatters;
using InfluxData.Net.InfluxDb.Infrastructure;

namespace InfluxData.Net.InfluxDb.RequestClients
{
    public class InfluxDbRequestClient_v_0_9_2 : InfluxDbRequestClient
    {
        public InfluxDbRequestClient_v_0_9_2(IInfluxDbClientConfiguration configuration) 
            : base(configuration)
        {
        }

        public override IPointFormatter GetPointFormatter()
        {
            return new PointFormatter_v_0_9_2();
        }
    }
}