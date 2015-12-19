using Newtonsoft.Json;

namespace InfluxData.Net.InfluxDb.Models.Responses
{
    /// <summary>
    /// Represents <see cref="{Database}"/> query response.
    /// </summary>
    public class DatabaseResponse
    {
        /// <summary>
        /// Database name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}