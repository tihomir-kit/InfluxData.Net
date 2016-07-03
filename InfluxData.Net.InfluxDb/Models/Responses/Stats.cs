using System.Collections.Generic;

namespace InfluxData.Net.InfluxDb.Models.Responses
{
    public class Stats
    {
        public IEnumerable<Serie> CQ { get; set; }

        public IEnumerable<Serie> Engine { get; set; }

        public IEnumerable<Serie> Shard { get; set; }

        public IEnumerable<Serie> Httpd { get; set; }

        public IEnumerable<Serie> WAL { get; set; }

        public IEnumerable<Serie> Write { get; set; }

        public IEnumerable<Serie> Runtime { get; set; }



        // Added to lib since InfluxDB v1.0.0.

        public IEnumerable<Serie> Database { get; set; }

        public IEnumerable<Serie> QueryExecutor { get; set; }

        public IEnumerable<Serie> Subscriber { get; set; }

        public IEnumerable<Serie> Tsm1Cache { get; set; }

        public IEnumerable<Serie> Tsm1Filestore { get; set; }

        public IEnumerable<Serie> Tsm1Wal { get; set; }
    }
}