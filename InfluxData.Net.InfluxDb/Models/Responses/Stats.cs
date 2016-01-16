using System;
using System.Collections;
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
    }
}