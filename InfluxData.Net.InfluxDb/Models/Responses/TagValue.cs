namespace InfluxData.Net.InfluxDb.Models.Responses
{
    public class TagValue
    {
        /// <summary>
        /// The tag's name/key.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The tag value.
        /// </summary>
        public string Value { get; set; }
    }
}
