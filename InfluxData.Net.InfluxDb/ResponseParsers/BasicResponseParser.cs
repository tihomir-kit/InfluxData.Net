using InfluxData.Net.InfluxDb.Helpers;
using InfluxData.Net.InfluxDb.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    public class BasicResponseParser : IBasicResponseParser
    {
        public IEnumerable<Serie> FlattenQueryResponseSeries(QueryResponse queryResponse)
        {
            var series = new List<Serie>();

            foreach (var result in queryResponse.Results)
            {
                series.AddRange(result.Series ?? new Serie[0]);
            }

            return series;
        }
    }
}
