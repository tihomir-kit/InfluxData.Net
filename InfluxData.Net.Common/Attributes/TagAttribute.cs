using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace InfluxData.Net.Common.Attributes
{
    public class TagAttribute : InfluxBaseAttribute
    {
        public TagAttribute([CallerMemberName]string name = null)
            : base(name) { }
    }
}
