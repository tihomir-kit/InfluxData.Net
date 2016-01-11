using InfluxData.Net.InfluxDb.Models.Responses;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;

namespace InfluxData.Net.InfluxDb.Infrastructure
{
    public static class InfluxDbResponseExtensions
    {
        public static T ReadAs<T>(this IInfluxDbApiResponse response)
        {
            return JsonConvert.DeserializeObject<T>(response.Body);
        }

        public static IInfluxDbApiResponse ValidateQueryResponse(this IInfluxDbApiResponse response)
        {
            response.ReadAs<QueryResponse>().Validate();
            return response;
        }

        public static QueryResponse Validate(this QueryResponse queryResponse)
        {
            Common.Helpers.Validate.IsNotNull(queryResponse, "queryResponse");
            Common.Helpers.Validate.IsNotNull(queryResponse.Results, "queryResponse.Results");

            if (queryResponse.Error != null)
            {
                throw new InfluxDbApiException(HttpStatusCode.BadRequest, queryResponse.Error);
            }

            // Apparently a 200 OK can return an error in the results
            // https://github.com/influxdb/influxdb/pull/1813
            var erroredResults = queryResponse.Results.Where(result => result.Error != null);
            foreach (var erroredResult in erroredResults)
            {
                throw new InfluxDbApiException(HttpStatusCode.BadRequest, erroredResult.Error);
            }

            return queryResponse;
        }
    }
}