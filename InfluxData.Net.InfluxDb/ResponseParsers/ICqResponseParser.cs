using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    public interface ICqResponseParser
    {
        IEnumerable<ContinuousQuery> GetContinuousQueries(string dbName, IEnumerable<Serie> series);
    }
}
