using System;
using System.Reflection;
using System.Text.RegularExpressions;
using InfluxData.Net.Common;
using System.Linq;

namespace InfluxData.Net.InfluxDb.Helpers
{
    public static class QueryHelpers
    {
        public static string BuildParameterizedQuery(string query, object param)
        {
            Type t = param.GetType();
            PropertyInfo[] pi = t.GetProperties();


            foreach (var propertyInfo in pi)
            {
                var regex = $@"@{propertyInfo.Name}(?!\w)";

                if(!Regex.IsMatch(query, regex) && Nullable.GetUnderlyingType(propertyInfo.GetType()) != null)
                    throw new ArgumentException($"Missing parameter identifier for @{propertyInfo.Name}");

                var paramValue = propertyInfo.GetValue(param);
                if (paramValue == null)
                    continue;

                var paramType = paramValue.GetType();

                if (!paramType.IsPrimitive && paramType != typeof(String) && paramType != typeof(DateTime))
                    throw new NotSupportedException($"The type {paramType.Name} is not a supported query parameter type.");

                var sanitizedParamValue = paramValue;

                if (paramType == typeof(String))
                {
                    sanitizedParamValue = ((string)sanitizedParamValue).Sanitize();
                }

                while (Regex.IsMatch(query, regex))
                {
                    var match = Regex.Match(query, regex);

                    query = query.Remove(match.Index, match.Length);
                    query = query.Insert(match.Index, $"{sanitizedParamValue}");
                }
            }

            return query;
        }
    }
}
