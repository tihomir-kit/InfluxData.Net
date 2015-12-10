using System;

namespace InfluxData.Net.Helpers
{
    internal static class HttpUtility
    {
        public static string UrlEncode(string parameter)
        {
            return Uri.EscapeUriString(parameter);
        }
    }
}