using System.Collections.Generic;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.InfluxDb.Constants;

namespace InfluxData.Net.InfluxDb.Infrastructure
{
    internal static class RequestParamsBuilder
    {
        public static IDictionary<string, string> BuildQueryRequestParams(string query)
        {
            return new Dictionary<string, string>
            {
                { QueryParams.Query, HttpUtility.UrlEncode(query) }
            };
        }

        public static IDictionary<string, string> BuildQueryRequestParams(string dbName, string query)
        {
            return BuildRequestParams(dbName, QueryParams.Query, query);
        }

        public static IDictionary<string, string> BuildRequestParams(string dbName, string paramKey, string paramValue)
        {
            return new Dictionary<string, string>
            {
                { QueryParams.Db, dbName },
                { paramKey, HttpUtility.UrlEncode(paramValue) }
            };
        }
    }
}
