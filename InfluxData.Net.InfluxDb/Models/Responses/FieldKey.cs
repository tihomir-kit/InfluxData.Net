namespace InfluxData.Net.InfluxDb.Models.Responses
{
    public class FieldKey
    {
        /// <summary>
        /// The field's name/key.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The field's data type (integer, string, float, etc.).
        /// </summary>
        public string Type { get; set; }
    }
}
