using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.Common.Helpers
{
    /// <summary>
    /// Used to describe <see cref="{TimeUnit}"/> write params used by InfluxDb.
    /// </summary>
    public class ParamValueAttribute : Attribute
    {
        private string _value;
        public string Value
        {
            get { return _value; }
        }

        public ParamValueAttribute(string value)
        {
            _value = value;
        }
    }
}
