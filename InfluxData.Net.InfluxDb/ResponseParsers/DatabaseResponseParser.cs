using InfluxData.Net.InfluxDb.Helpers;
using InfluxData.Net.InfluxDb.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    public class DatabaseResponseParser : IDatabaseResponseParser
    {
        public IEnumerable<Database> GetDatabases(QueryResponse queryResponse)
        {
            var databases = new List<Database>();

            var series = queryResponse.Results.Single().Series;
            if (series == null)
                return databases;

            databases.AddRange(series.Single().Values.Select(p => new Database()
            {
                Name = (string)p[0]
            }));

            return databases;
        }
    }
}
