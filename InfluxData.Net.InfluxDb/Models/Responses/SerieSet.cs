using System.Collections.Generic;

namespace InfluxData.Net.InfluxDb.Models.Responses
{
    /// <summary>
    /// Represents a serie set (obtained by "SHOW SERIES").
    /// </summary>
    public class SerieSet
    {
        public SerieSet()
        {
            Series = new List<SerieSetItem>();
        }

        /// <summary>
        /// Serie/measurement name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Collection of serie set items.
        /// </summary>
        public IList<SerieSetItem> Series { get; set; }
    }

    public class SerieSetItem
    {
        public SerieSetItem()
        {
            Tags = new Dictionary<string, string>();
        }

        /// <summary>
        /// Serie set item key (defined by "_key").
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Serie set item tags.
        /// </summary>
        public IDictionary<string, string> Tags { get; set; }
    }
}