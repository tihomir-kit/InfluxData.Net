using System.Collections.Generic;
using InfluxData.Net.Kapacitor.Constants;
using Newtonsoft.Json;

namespace InfluxData.Net.Kapacitor.Models
{
    /// <summary>
    /// Task definition object. Used for creating tasks in Kapacitor. Uses a task template.
    /// </summary>
    public class TemplateTaskParams : BaseTaskParams
    {
        /// <summary>
        /// The Id of the template to use for creating this task.
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// Variable dictionary that will be passed to create this task.
        /// </summary>
        public Dictionary<string, TemplateVar> TemplateVars { get; set; }
    }

    /// <summary>
    /// Represents a variable that is used for creating a task with template.
    /// </summary>
    public class TemplateVar
    {
        /// <summary>
        /// The type of the variable (lambda, string, etc..).
        /// </summary>
        [JsonProperty("type")] // TOOD: check if this is needed
        public string Type { get; set; }

        /// <summary>
        /// The value for the task.
        /// </summary>
        [JsonProperty("value")] // TOOD: check if this is needed
        public string Value { get; set; }
    }
}