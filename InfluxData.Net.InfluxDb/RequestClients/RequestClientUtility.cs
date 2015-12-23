using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Constants;

namespace InfluxData.Net.InfluxDb.RequestClients
{
    internal static class RequestClientUtility
    {
        internal static IDictionary<string, string> BuildQueryRequestParams(string query)
        {
            return new Dictionary<string, string>
            {
                { QueryParams.Query, query }
            };
        }

        internal static IDictionary<string, string> BuildQueryRequestParams(string dbName, string query)
        {
            return BuildRequestParams(dbName, QueryParams.Query, query);
        }

        internal static IDictionary<string, string> BuildRequestParams(string dbName, string paramKey, string paramValue)
        {
            return new Dictionary<string, string>
            {
                { QueryParams.Db, dbName },
                { paramKey, paramValue }
            };
        }
    }
}
