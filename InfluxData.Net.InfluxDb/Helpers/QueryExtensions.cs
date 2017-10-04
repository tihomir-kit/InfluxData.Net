using InfluxData.Net.Common;
using System;
using System.Text.RegularExpressions;

namespace InfluxData.Net.InfluxDb.Helpers
{
    public static class QueryExtensions
    {
        public static string BuildParameterizedQuery(string query, object parameters)
        {
            var type = parameters.GetType();
            var properties = type.GetProperties();

            foreach (var propertyInfo in properties)
            {
                var regex = $@"@{propertyInfo.Name}(?!\w)";

                if(!Regex.IsMatch(query, regex) && Nullable.GetUnderlyingType(propertyInfo.GetType()) != null)
                    throw new ArgumentException($"Missing parameter identifier for @{propertyInfo.Name}");

                var paramValue = propertyInfo.GetValue(parameters);
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
