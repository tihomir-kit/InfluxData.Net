using Newtonsoft.Json;

namespace InfluxData.Net.InfluxDb.Infrastructure
{
    public static class InfluxDbResponseExtensions
    {
        public static T ReadAs<T>(this IInfluxDbApiResponse response)
        {
            var @object = JsonConvert.DeserializeObject<T>(response.Body);

            return @object;
        }
    }
}