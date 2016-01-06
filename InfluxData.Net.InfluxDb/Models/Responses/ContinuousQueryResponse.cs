using Newtonsoft.Json;

namespace InfluxData.Net.InfluxDb.Models.Responses
{
    /// <summary>
    /// Represents <see cref="{ContinuousQueryResponse}"/> query response.
    /// </summary>
    public class ContinuousQueryResponse
    {
        /// <summary>
        /// CQ name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// CQ query.
        /// </summary>
        public string Query { get; set; }
    }
}