using System;
using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.Kapacitor.Constants;

namespace InfluxData.Net.Kapacitor.Models
{
    /// <summary>
    /// abstract Task definition object. Used for creating tasks in Kapacitor.
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
            get { return this.TaskId; }
            set { this.TaskId = value; }
        }        

        /// <summary>
        /// Database name / retention policy params.
        /// </summary>
        public DBRPsParams DBRPsParams { get; set; }

        /// <summary>
        /// Generates a json mapping dictionary from type to value
        /// </summary>
        /// <returns></returns>
        internal virtual Dictionary<string, object> ToJsonDictionary()
        {
            var jsonDictionary = new Dictionary<string, object>
            {
                {BodyParams.Id, TaskId},               
                {
                    BodyParams.Dbrps, new List<IDictionary<string, string>>
                    {
                        new Dictionary<string, string>()
                        {
                            {BodyParams.Db, DBRPsParams.DbName},
                            {BodyParams.RetentionPolicy, DBRPsParams.RetentionPolicy}
                        }
                    }
                }
            };
            return jsonDictionary;
        }
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