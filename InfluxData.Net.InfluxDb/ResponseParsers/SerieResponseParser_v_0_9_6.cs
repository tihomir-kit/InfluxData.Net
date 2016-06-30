using System.Collections.Generic;
using System.Linq;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    internal class SerieResponseParser_v_0_9_6 : SerieResponseParser
    {
        protected override string KeyColumnName
        {
            get { return "_key"; }
        }
    }
}
