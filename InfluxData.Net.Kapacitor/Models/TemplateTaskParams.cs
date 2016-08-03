using System.Collections.Generic;
using InfluxData.Net.Kapacitor.Constants;
using Newtonsoft.Json;

namespace InfluxData.Net.Kapacitor.Models
{
    /// <summary>
    /// Task definition object. Used for creating tasks in Kapacitor. using a given task template
    /// </summary>
    public class TemplateTaskParams : BaseTaskParams
    {
        /// <summary>
        /// the id of the template to use for creating this task
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// the variable dictionary that will be passed to create this task
        /// </summary>
        public Dictionary<string, TemplateVar> TemplateVars { get; set; }

        internal override Dictionary<string, object> ToJsonDictionary()
        {
            var jsonDictionary = base.ToJsonDictionary();

            jsonDictionary.Add(BodyParams.TemplateId, TemplateId);

            jsonDictionary.Add(BodyParams.TemplateVars, TemplateVars);

            return jsonDictionary;
        }
    }

    /// <summary>
    /// Represents a variable that is used for creating a task with template
    /// </summary>
    public class TemplateVar
    {
        /// <summary>
        /// the type of the variable (lambda, string, etc..)
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// the value for the task
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}