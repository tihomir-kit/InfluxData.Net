using System;
using System.Collections.Generic;

namespace InfluxData.Net.Kapacitor.Models.Responses
{
    public class KapacitorTasks
    {
        public IEnumerable<KapacitorTask> Tasks { get; set; }
    }

    public class KapacitorTask
    {
        public string Id { get; set; }

        [Obsolete("Please use Id property instead")]
        public string Name
        {
            get
            {
                return this.Id;
            }
            set
            {
                this.Id = value;
            }
        }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public string Type { get; set; }

        public IEnumerable<DBRPs> DBRPs { get; set; }

        public string Script { get; set; }

        [Obsolete("Please use Id property instead")]
        public string TICKscript
        {
            get
            {
                return this.Script;
            }
            set
            {
                this.Script = value;
            }
        }

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
