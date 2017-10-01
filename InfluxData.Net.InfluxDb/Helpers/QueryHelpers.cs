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
            var paramRegex = "@([A-Za-z0-9åäöÅÄÖ'_-]+)";

            var matches = Regex.Matches(query, paramRegex);

            Type t = param.GetType();
            PropertyInfo[] pi = t.GetProperties();

            foreach(Match match in matches) 
            {
                if (!pi.Any(x => match.Groups[0].Value.Contains(x.Name)))
                    throw new ArgumentException($"Missing parameter value for {match.Groups[0].Value}");
            }

            foreach (var propertyInfo in pi)
            {
                var paramValue = propertyInfo.GetValue(param);
                var paramType = paramValue.GetType();

                if(!paramType.IsPrimitive && paramType != typeof(String))
                    throw new NotSupportedException($"The type {paramType.Name} is not a supported query parameter type.");

                var sanitizedParamValue = paramValue;

                if(paramType == typeof(String)) {
                    sanitizedParamValue = ((string)sanitizedParamValue).Sanitize();
                }

                while (Regex.IsMatch(query, $"@{propertyInfo.Name}"))
                {
                    var match = Regex.Match(query, $"@{propertyInfo.Name}");

                    query = query.Remove(match.Index, match.Length);
                    query = query.Insert(match.Index, $"{sanitizedParamValue}");
                }
            }

            return query;
        }
    }
}