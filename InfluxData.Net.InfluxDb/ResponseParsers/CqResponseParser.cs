using InfluxData.Net.InfluxDb.Helpers;
using InfluxData.Net.InfluxDb.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InfluxData.Net.InfluxDb.ResponseParsers
{
    public class CqResponseParser : ICqResponseParser
    {
        public IEnumerable<ContinuousQuery> GetContinuousQueries(string dbName, QueryResponse queryResponse)
        {
            var cqs = new List<ContinuousQuery>();

            var series = queryResponse.Results.Single().Series; // TODO: test if InfluxDB ever returns null '.Series'
            if (series == null)
                return cqs;

            var serie = series.FirstOrDefault(p => p.Name == dbName);
            if (serie == null || serie.Values == null)
                return cqs;

            var columns = serie.Columns.ToArray();
            var indexOfName = Array.IndexOf(columns, "name");
            var indexOfQuery = Array.IndexOf(columns, "query");

            cqs.AddRange(serie.Values.Select(p => new ContinuousQuery()
            {
                Name = (string)p[indexOfName],
                Query = (string)p[indexOfQuery]
            }));

            return cqs;
        }
    }
}
