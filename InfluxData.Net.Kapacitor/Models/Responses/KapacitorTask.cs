using System.Collections.Generic;

namespace InfluxData.Net.Kapacitor.Models.Responses
{
    public class KapacitorTask
    {
        public string Name { get; set; }

        public int Type { get; set; }

        public IList<DBRPs> DBRPs { get; set; }

        public string TICKscript { get; set; }

        public string Dot { get; set; }

        public bool Enabled { get; set; }

        public bool Executing { get; set; }

        public string Error { get; set; }
    }

    public class DBRPs
    {
        public string Db { get; set; }

        public string Rp { get; set; }
    }
}
