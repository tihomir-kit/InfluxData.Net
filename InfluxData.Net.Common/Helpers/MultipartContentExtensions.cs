using System.Net.Http;

namespace InfluxData.Net.Common.Helpers
{
    public static class MultipartContentExtensions
    {
        public static MultipartFormDataContent ToMultipartHttpContent(this string content, string name)
        {
            var httpContent = new MultipartFormDataContent();
            httpContent.Add(new StringContent(content), name);

            return httpContent;
        }
    }
}
