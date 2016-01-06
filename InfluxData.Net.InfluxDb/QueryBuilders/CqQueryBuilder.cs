using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.InfluxDb.Enums;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.Common.Helpers;

namespace InfluxData.Net.InfluxDb.QueryBuilders
{
    internal class CqQueryBuilder : ICqQueryBuilder
    {
        public string CreateContinuousQuery(CqParams cqParams)
        {
            var downsamplers = cqParams.Downsamplers.ToCommaSpaceSeparatedString();
            var tags = BuildTags(cqParams.Tags);
            var fillType = BuildFillType(cqParams.FillType);

            var subQuery = String.Format(QueryStatements.CreateContinuousQuerySubQuery,
                downsamplers, cqParams.DsSerieName, cqParams.SourceSerieName, cqParams.Interval, tags, fillType);

            var query = String.Format(QueryStatements.CreateContinuousQuery, cqParams.CqName, cqParams.DbName, subQuery);

            return query;
        }

        public string GetContinuousQueries()
        {
            return QueryStatements.GetContinuousQueries;
        }

        public string DeleteContinuousQuery(string dbName, string cqName)
        {
            var query = String.Format(QueryStatements.DropContinuousQuery, cqName, dbName);

            return query;
        }

        public string Backfill(string dbName, BackfillParams backfill)
        {
            var downsamplers = backfill.Downsamplers.ToCommaSpaceSeparatedString();
            var filters = BuildFilters(backfill.Filters);
            var timeFrom = backfill.TimeFrom.ToString("yyyy-MM-dd HH:mm:ss");
            var timeTo = backfill.TimeTo.ToString("yyyy-MM-dd HH:mm:ss");
            var tags = BuildTags(backfill.Tags);
            var fillType = BuildFillType(backfill.FillType);

            var query = String.Format(QueryStatements.Backfill, 
                downsamplers, backfill.DsSerieName, backfill.SourceSerieName, filters, timeFrom, timeTo, backfill.Interval, tags, fillType);

            return query;
        }

        private static string BuildFilters(IEnumerable<string> filters)
        {
            return filters == null ? String.Empty : String.Join(" ", filters.ToAndSpaceSeparatedString(), "AND");
        }

        private static string BuildTags(IEnumerable<string> tags)
        {
            return tags == null ? String.Empty : String.Join(" ", ",", tags.ToCommaSpaceSeparatedString());
        }

        private static string BuildFillType(FillType fillType)
        {
            return fillType == FillType.Null ? String.Empty : String.Format(QueryStatements.Fill, fillType.ToString().ToLower());
        }
    }
}
