using System;

namespace InfluxData.Net.Common.Helpers
{
    public static class HttpUtility
    {
        public static string UrlEncode(string parameter)
        {
            return Uri.EscapeUriString(parameter);
        }
    }
}