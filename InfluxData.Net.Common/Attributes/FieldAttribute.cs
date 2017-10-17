using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace InfluxData.Net.Common.Attributes
{
    public class FieldAttribute : InfluxBaseAttribute
    {
        public FieldAttribute([CallerMemberName]string name = null)
            : base(name) { }
    }
}
