using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.Enums
{
    /// <summary>
    /// Different user database permissions.
    /// </summary>
    public enum Privileges
    {
        None = 0,
        Read = 1,
        Write = 2,
        All = 4,
    }
}
