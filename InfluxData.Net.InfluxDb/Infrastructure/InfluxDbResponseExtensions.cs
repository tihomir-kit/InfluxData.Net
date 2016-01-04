using Newtonsoft.Json;

namespace InfluxData.Net.InfluxDb.Infrastructure
{
    public static class InfluxDbResponseExtensions
    {
        public static T ReadAs<T>(this IInfluxDbApiResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Body);
        }
    }
}