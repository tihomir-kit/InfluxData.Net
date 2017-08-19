using Newtonsoft.Json;
using InfluxData.Net.Common.Infrastructure;

namespace InfluxData.Net.InfluxData.Helpers
{
    public static class ResponseExtensionsBase
    {
        public static T ReadAs<T>(this IInfluxDataApiResponse response)
        {
            return response.Body.ReadAs<T>();
        }

        public static T ReadAs<T>(this string responseBody)
        {
            return JsonConvert.DeserializeObject<T>(responseBody);
        }
    }
}