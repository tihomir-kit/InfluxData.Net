using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfluxData.Net.InfluxDb.Enums;

namespace InfluxData.Net.Kapacitor.Models
{
    public class DefineTaskParams
    {
        public string TaskName { get; set; }

        public TaskType TaskType { get; set; }

        public DbrpsParams DbrpsParams { get; set; }

        public string TickScript { get; set; }
    }

    public class DbrpsParams
    {
        public string DbName { get; set; }

        public string RetentionPolicy { get; set; }
    }
}
