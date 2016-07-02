using InfluxData.Net.InfluxDb.Enums;
using System;

namespace InfluxData.Net.Kapacitor.Models
{
    public class DefineTaskParams
    {
        /// <summary>
        /// Task Id (Name in older versions).
        /// </summary>
        public string TaskId { get; set; }

        [Obsolete("Please use TaskId property instead")]
        public string TaskName
        {
            get
            {
                return this.TaskId;
            }
            set
            {
                this.TaskId = value;
            }
        }

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
