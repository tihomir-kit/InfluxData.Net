using InfluxData.Net.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InfluxData.Net.Enums;

namespace InfluxData.Net.Infrastructure.Clients
{
    internal static class InfluxDbClientUtility
    {
        public static Dictionary<string, string> BuildQueryRequestParams(string query)
        {
            return new Dictionary<string, string>
            {
                { QueryParams.Query, query }
            };
        }

        public static Dictionary<string, string> BuildQueryRequestParams(string dbName, string query)
        {
            return BuildRequestParams(dbName, QueryParams.Query, query);
        }

        public static Dictionary<string, string> BuildRequestParams(string dbName, string requestType, string query)
        {
            return new Dictionary<string, string>
            {
                { QueryParams.Db, dbName },
                { requestType, query }
            };
        }
    }
}
