using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    public interface IDatabaseResponseParser
    {
        IEnumerable<Database> GetDatabases(IEnumerable<Serie> series);
    }
}
