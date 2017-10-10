using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.Kapacitor.Models;

namespace InfluxData.Net.Integration.Kapacitor
{
    public class IntegrationFixture_v_0_10_1 : IntegrationFixtureBase
    {
        public IntegrationFixture_v_0_10_1()
            :base ("InfluxDbEndpointUri_v_0_9_6", InfluxDbVersion.v_0_9_6, "KapacitorEndpointUri_v_0_10_1", KapacitorVersion.v_0_10_1)
        {
        }

        public override DefineTaskParams MockDefineTaskParams()
        {
            return new DefineTaskParams()
            {
                TaskId = CreateRandomTaskId(),
                TaskType = TaskType.Stream,
                DBRPsParams = new DBRPsParams()
                {
                    DbName = this.DbName,
                    RetentionPolicy = "default"
                },
                TickScript = "stream\r\n" +
                             "    .from().measurement('reading')\r\n" +
                             "    .alert()\r\n" +
                             "        .crit(lambda: \"Humidity\" < 36)\r\n" +
                             "        .log('/tmp/alerts.log')\r\n"
            };
        }
    }
}