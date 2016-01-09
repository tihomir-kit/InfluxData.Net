using System;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Infrastructure;
using System.Collections.Generic;
using InfluxData.Net.Common.Helpers;
using System.Linq;

namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    internal class SerieQueryBuilder : ISerieQueryBuilder
    {
        public string GetSeries(string dbName, string measurementName = null, IEnumerable<string> filters = null)
        {
            var query = QueryStatements.GetSeries;

            if (!String.IsNullOrEmpty(measurementName))
            {
                query = String.Join(" FROM ", query, measurementName);
            }

            if (filters != null && filters.Any())
            {
                query = String.Join(" WHERE ", query, filters.ToCommaSpaceSeparatedString());
            }

            return query;
        }

        public string DropSeries(string dbName, string measurementName, IEnumerable<string> filters = null)
        {
            return DropSeries(dbName, new List<string>() { measurementName }, filters);
        }

        public string DropSeries(string dbName, IEnumerable<string> measurementNames, IEnumerable<string> filters = null)
        {
            var query = String.Format(QueryStatements.DropSeries, measurementNames.ToCommaSeparatedString());

            if (filters != null && filters.Any())
            {
                query = String.Join(" WHERE ", query, filters.ToCommaSpaceSeparatedString());
            }

            return query;
        }

        public string GetMeasurements(string dbName, string withClause = null, IEnumerable<string> filters = null)
        {
            var query = QueryStatements.GetMeasurements;

            if (!String.IsNullOrEmpty(withClause))
            {
                query = String.Join(" WITH MEASUREMENT ", query, withClause);
            }

            if (filters != null && filters.Any())
            {
                query = String.Join(" WHERE ", query, filters.ToCommaSpaceSeparatedString());
            }

            return query;
        }

        public string DropMeasurement(string dbName, string measurementName)
        {
            var query = String.Format(QueryStatements.DropMeasurement, measurementName);

            return query;
        }
    }
}
