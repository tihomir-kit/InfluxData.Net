using InfluxData.Net.InfluxDb.Enums;

namespace InfluxData.Net.Kapacitor.Models
{
    public class DefineTaskParams
    {
        public string TaskName { get; set; }

        public TaskType TaskType { get; set; }

        public DBRPsParams DBRPsParams { get; set; }

        public string TickScript { get; set; }
    }

    public class DBRPsParams
    {
        public string DbName { get; set; }

        public string RetentionPolicy { get; set; }
    }
}
