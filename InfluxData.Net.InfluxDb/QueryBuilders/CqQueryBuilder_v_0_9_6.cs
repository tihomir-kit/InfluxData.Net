using System;
using InfluxData.Net.InfluxDb.Models;

namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    internal class CqQueryBuilder_v_0_9_6 : CqQueryBuilder
    {
        protected override string BuildResample(CqResampleParam resampleParam)
        {
            if (!String.IsNullOrEmpty(resampleParam.For) || !String.IsNullOrEmpty(resampleParam.Every))
                throw new NotSupportedException("Resampling is not supported by this version of InfluxDB");

            return String.Empty;
        }
    }
}
