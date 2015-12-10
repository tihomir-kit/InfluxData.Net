using Newtonsoft.Json;

namespace InfluxData.Net.Models
{
    public class Database
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}