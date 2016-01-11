using InfluxData.Net.InfluxDb.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    public interface IBasicResponseParser
    {
        IEnumerable<Serie> FlattenResultsSeries(IEnumerable<SeriesResult> seriesResults);

        IEnumerable<IEnumerable<Serie>> MapResultsSeries(IEnumerable<SeriesResult> seriesResults);
    }
}
