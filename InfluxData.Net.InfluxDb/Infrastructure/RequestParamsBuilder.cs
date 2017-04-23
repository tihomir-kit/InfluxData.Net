using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Constants;
using InfluxData.Net.Common.Helpers;
using System;

namespace InfluxData.Net.InfluxDb.Infrastructure
{
    internal static class RequestParamsBuilder
    {
        public static IDictionary<string, string> BuildQueryRequestParams(string query = null, string dbName = null, string epochFormat = null, long? chunkSize = null)
        {
            var requestParams = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(dbName))
                requestParams.Add(QueryParams.Db, HttpUtility.UrlEncode(dbName));

            if (!String.IsNullOrEmpty(query))
                requestParams.Add(QueryParams.Query, HttpUtility.UrlEncode(query));

            if (!String.IsNullOrEmpty(epochFormat))
                requestParams.Add(QueryParams.Epoch, HttpUtility.UrlEncode(epochFormat));

            if (chunkSize != null)
            {
                requestParams.Add(QueryParams.Chunked, "true");
                requestParams.Add(QueryParams.ChunkSize, chunkSize.ToString());
            }

            return requestParams;
        }

        public static IDictionary<string, string> BuildRequestParams(string dbName, string paramKey1, string paramValue1, string paramKey2, string paramValue2)
        {
            var requestParams = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(dbName))
                requestParams.Add(QueryParams.Db, HttpUtility.UrlEncode(dbName));
            if (paramKey1 != null && paramValue1 != null)
                requestParams.Add(paramKey1, HttpUtility.UrlEncode(paramValue1));
            if (paramKey2 != null && paramValue2 != null)
                requestParams.Add(paramKey2, HttpUtility.UrlEncode(paramValue2));

            return requestParams;
        }
    }
}
