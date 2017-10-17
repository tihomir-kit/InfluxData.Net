﻿using System;
using System.Collections.Generic;
using System.Text;

namespace InfluxData.Net.Common.Attributes
{
    public class TimestampAttribute : InfluxBaseAttribute
    {
        public TimestampAttribute()
            : base("time") { }
    }
}
