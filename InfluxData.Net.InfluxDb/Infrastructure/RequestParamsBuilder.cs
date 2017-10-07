using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Constants;
using System;

namespace InfluxData.Net.InfluxDb.Infrastructure
{
    internal static class RequestParamsBuilder
    {
        public static IDictionary<string, string> BuildQueryRequestParams(string query = null, string dbName = null, string epochFormat = null, long? chunkSize = null)
        {
            var requestParams = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(dbName))
                requestParams.Add(QueryParams.Db, Uri.EscapeDataString(dbName));

            if (!String.IsNullOrEmpty(query))
                requestParams.Add(QueryParams.Query, Uri.EscapeDataString(query));

            if (!String.IsNullOrEmpty(epochFormat))
                requestParams.Add(QueryParams.Epoch, Uri.EscapeDataString(epochFormat));

            if (chunkSize != null)
            {
                requestParams.Add(QueryParams.Chunked, "true");
                requestParams.Add(QueryParams.ChunkSize, chunkSize.ToString());
            }

            return requestParams;
        }

        public static IDictionary<string, string> BuildRequestParams(string dbName = null, string epochFormat = null, long? chunkSize = null)
        {
            return BuildQueryRequestParams(null, dbName, epochFormat, chunkSize);
        }

        public static IDictionary<string, string> BuildRequestParams(string dbName, string paramKey1, string paramValue1, string paramKey2, string paramValue2)
        {
            var requestParams = new Dictionary<string, string>();

            if (!String.IsNullOrEmpty(dbName))
                requestParams.Add(QueryParams.Db, Uri.EscapeDataString(dbName));
            if (paramKey1 != null && paramValue1 != null)
                requestParams.Add(paramKey1, Uri.EscapeDataString(paramValue1));
            if (paramKey2 != null && paramValue2 != null)
                requestParams.Add(paramKey2, Uri.EscapeDataString(paramValue2));

            return requestParams;
        }
    }
}
