using InfluxData.Net.InfluxDb.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    public interface ISerieResponseParser
    {
        IEnumerable<SerieSet> GetSerieSets(IEnumerable<Serie> series);

        IEnumerable<Measurement> GetMeasurements(QueryResponse queryResponse);
    }
}
