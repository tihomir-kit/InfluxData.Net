using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    public interface IBasicResponseParser
    {
        IEnumerable<Serie> FlattenResultsSeries(IEnumerable<SeriesResult> seriesResults);

        IEnumerable<IEnumerable<Serie>> MapResultsSeries(IEnumerable<SeriesResult> seriesResults);
    }
}
