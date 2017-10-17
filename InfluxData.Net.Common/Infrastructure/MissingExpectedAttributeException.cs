using System;
using System.Collections.Generic;
using System.Text;

namespace InfluxData.Net.Common.Infrastructure
{
    public class MissingExpectedAttributeException : Exception
    {
        public MissingExpectedAttributeException(Type attributeType)
            : base($"The expected attribute: {attributeType.Name} is missing")
        {
        }
    }
}
