using System;
using System.Collections.Generic;
using System.Text;

namespace InfluxData.Net.Common.Attributes
{
    public class InfluxBaseAttribute : Attribute
    {

        public InfluxBaseAttribute() { }

        public InfluxBaseAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}
