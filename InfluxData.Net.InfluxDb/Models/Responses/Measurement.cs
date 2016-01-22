namespace InfluxData.Net.InfluxDb.Models.Responses
{
    /// <summary>
    /// Represents <see cref="{Measurement}"/> query response.
    /// </summary>
    public class Measurement
    {
        /// <summary>
        /// Database name.
        /// </summary>
        public string Name { get; set; }
    }
}