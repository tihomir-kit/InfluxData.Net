using InfluxData.Net.InfluxDb.Enums;
using System;

namespace InfluxData.Net.Kapacitor.Models
{
    /// <summary>
    /// Task definition object. Used for creating tasks in Kapacitor.
    /// </summary>
    public class DefineTaskParams
    {
        /// <summary>
        /// Task id (Name in older versions).
        /// </summary>
        public string TaskId { get; set; }

        /// <summary>
        /// Task name.
        /// </summary>
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

        /// <summary>
        /// Task type - stream, batch..
        /// </summary>
        public TaskType TaskType { get; set; }

        /// <summary>
        /// Database name / retention policy params.
        /// </summary>
        public DBRPsParams DBRPsParams { get; set; }

        /// <summary>
        /// Tick script to save.
        /// </summary>
        public string TickScript { get; set; }
    }

    /// <summary>
    /// Database name / retention policy params object. Used as a part of DefineTaskParams.
    /// </summary>
    public class DBRPsParams
    {
        /// <summary>
        /// Database name.
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// Retention policy.
        /// </summary>
        public string RetentionPolicy { get; set; }
    }
}
