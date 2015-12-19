using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfluxData.Net.InfluxDb.Constants;

namespace InfluxData.Net.InfluxDb.RequestClients
{
    internal static class RequestClientUtility
    {
        internal static Dictionary<string, string> BuildQueryRequestParams(string query)
        {
            return new Dictionary<string, string>
            {
                { QueryParams.Query, query }
            };
        }

        internal static Dictionary<string, string> BuildQueryRequestParams(string dbName, string query)
        {
            return BuildRequestParams(dbName, QueryParams.Query, query);
        }

        internal static Dictionary<string, string> BuildRequestParams(string dbName, string paramKey, string paramValue)
        {
            return new Dictionary<string, string>
            {
                { QueryParams.Db, dbName },
                { paramKey, paramValue }
            };
        }
    }
}
