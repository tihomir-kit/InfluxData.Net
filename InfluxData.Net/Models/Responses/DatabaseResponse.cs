using Newtonsoft.Json;

namespace InfluxData.Net.Models.Responses
{
    public class DatabaseResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}