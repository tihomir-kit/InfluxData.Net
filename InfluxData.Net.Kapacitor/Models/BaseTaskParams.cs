using System;
using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.Kapacitor.Constants;

namespace InfluxData.Net.Kapacitor.Models
{
    /// <summary>
    /// Base Task definition object. Used for creating tasks in Kapacitor.
    /// </summary>
    public abstract class BaseTaskParams
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
            set {
                this.TaskId = value;
            }
        }

        /// <summary>
        /// Database name / retention policy params.
        /// </summary>
        public DBRPsParams DBRPsParams { get; set; }
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