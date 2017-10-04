using InfluxData.Net.Common;
using System;
using System.Text.RegularExpressions;

namespace InfluxData.Net.InfluxDb.Helpers
{
    public static class QueryExtensions
    {
        /// <summary>
        /// Builds a parametarized query from a query template and parameters object (uses reflection).
        /// </summary>
        /// <param name="queryTemplate">Query template to use.</param>
        /// <param name="parameters">Dynamic parameters object.</param>
        /// <returns>Full query.</returns>
        public static string BuildQuery(this string queryTemplate, object parameters)
        {
            var type = parameters.GetType();
            var properties = type.GetProperties();

            foreach (var propertyInfo in properties)
            {
                var regex = $@"@{propertyInfo.Name}(?!\w)";

                if(!Regex.IsMatch(queryTemplate, regex) && Nullable.GetUnderlyingType(propertyInfo.GetType()) != null)
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

                while (Regex.IsMatch(queryTemplate, regex))
                {
                    var match = Regex.Match(queryTemplate, regex);

                    queryTemplate = queryTemplate.Remove(match.Index, match.Length);
                    queryTemplate = queryTemplate.Insert(match.Index, $"{sanitizedParamValue}");
                }
            }

            return queryTemplate;
        }
    }
}
