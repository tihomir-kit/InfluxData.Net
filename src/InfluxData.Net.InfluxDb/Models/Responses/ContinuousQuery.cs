namespace InfluxData.Net.InfluxDb.Models.Responses
{
    /// <summary>
    /// Represents <see cref="{ContinuousQuery}"/> query response.
    /// </summary>
    public class ContinuousQuery
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