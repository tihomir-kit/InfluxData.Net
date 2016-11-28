using System;
using System.Collections.Generic;
using System.Linq;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Constants;

namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    internal class SerieQueryBuilder : ISerieQueryBuilder
    {
        public virtual string GetSeries(string dbName, string measurementName = null, IEnumerable<string> filters = null)
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

        public virtual string DropSeries(string dbName, string measurementName, IEnumerable<string> filters = null)
        {
            return DropSeries(dbName, new List<string>() { measurementName }, filters);
        }

        public virtual string DropSeries(string dbName, IEnumerable<string> measurementNames, IEnumerable<string> filters = null)
        {
            var query = String.Format(QueryStatements.DropSeries, measurementNames.ToCommaSeparatedString());

            if (filters != null && filters.Any())
            {
                query = String.Join(" WHERE ", query, filters.ToCommaSpaceSeparatedString());
            }

            return query;
        }

        public virtual string GetMeasurements(string dbName, IEnumerable<string> filters = null)
        {
            var query = QueryStatements.GetMeasurements;

            if (filters != null && filters.Any())
            {
                query = String.Join(" WHERE ", query, filters.ToCommaSpaceSeparatedString());
            }

            return query;
        }

        public virtual string DropMeasurement(string dbName, string measurementName)
        {
            var query = String.Format(QueryStatements.DropMeasurement, measurementName);

            return query;
        }

        public virtual string GetTagKeys(string dbName, string measurementName)
        {
            var query = String.Format(QueryStatements.ShowTagKeys, measurementName);

            return query;
        }

        public virtual string GetTagValues(string dbName, string measurementName, string tagName)
        {
            var query = String.Format(QueryStatements.ShowTagValues, measurementName, tagName);

            return query;
        }

        public virtual string GetFieldKeys(string dbName, string measurementName)
        {
            var query = String.Format(QueryStatements.ShowFieldKeys, measurementName);

            return query;
        }
    }
}
