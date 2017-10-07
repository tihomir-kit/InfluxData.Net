using System;
using System.Text.RegularExpressions;

namespace InfluxData.Net.Common
{
    public static class StringExtensions
    {
        // http://www.mvvm.ro/2011/03/sanitize-strings-against-sql-injection.html
        public static string Sanitize(this string stringValue)
        {
            if (null == stringValue)
                return stringValue;

            return stringValue
                .RegexReplace("-{2,}", "-")
                .RegexReplace(@"[*/]+", String.Empty)
                .RegexReplace(@"(;|\s)(exec|execute|select|insert|update|delete|create|alter|drop|rename|truncate|backup|restore)\s", 
                    String.Empty, RegexOptions.IgnoreCase);
        }

        private static string RegexReplace(this string stringValue, string matchPattern, string toReplaceWith)
        {
            return Regex.Replace(stringValue, matchPattern, toReplaceWith);
        }

        private static string RegexReplace(this string stringValue, string matchPattern, string toReplaceWith, RegexOptions regexOptions)
        {
            return Regex.Replace(stringValue, matchPattern, toReplaceWith, regexOptions);
        }

    }
}
