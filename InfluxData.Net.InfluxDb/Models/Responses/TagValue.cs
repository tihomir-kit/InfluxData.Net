using System.Collections.Generic;


namespace InfluxData.Net.InfluxDb.Models.Responses
{
    public class TagValue
    {
        public string Name { get; set; }

        public object Value { get; set; }
    }
}
