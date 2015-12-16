using Newtonsoft.Json;

namespace InfluxData.Net.InfluxDb.Models.Responses
{
    public class DatabaseResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}