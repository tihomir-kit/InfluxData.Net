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
            return BuildRequestParams(dbName, paramKey, paramValue, null, null);
        }

        public static IDictionary<string, string> BuildRequestParams(string dbName, string paramKey1, string paramValue1, string paramKey2, string paramValue2)
        {
            var dict = new Dictionary<string, string>
            {
                {QueryParams.Db, dbName}
            };
            if (paramKey1 != null && paramValue1 != null)
                dict.Add(paramKey1, paramValue1);
            if (paramKey2 != null && paramValue2 != null)
                dict.Add(paramKey2, paramValue2);
            return dict;
        }
    }
}
