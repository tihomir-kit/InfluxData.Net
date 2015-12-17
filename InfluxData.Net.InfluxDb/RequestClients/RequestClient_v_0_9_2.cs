using InfluxData.Net.InfluxDb.Formatters;
using InfluxData.Net.InfluxDb.Infrastructure;

namespace InfluxData.Net.InfluxDb.RequestClients
{
    internal class RequestClient_v_0_9_2 : RequestClient
    {
        public RequestClient_v_0_9_2(IInfluxDbClientConfiguration configuration) 
            : base(configuration)
        {
        }

        public override IInfluxDbFormatter GetFormatter()
        {
            return new Formatter_v_0_9_2();
        }
    }
}