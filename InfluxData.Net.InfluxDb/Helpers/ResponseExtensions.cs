using System.Linq;
using System.Net;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxData.Helpers;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.Helpers
{
    public static class ResponseExtensions
    {
        public static IInfluxDataApiResponse ValidateQueryResponse(this IInfluxDataApiResponse response)
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
                throw new InfluxDataApiException(HttpStatusCode.BadRequest, queryResponse.Error);
            }

            // Apparently a 200 OK can return an error in the results
            // https://github.com/influxdb/influxdb/pull/1813
            var erroredResults = queryResponse.Results.Where(result => result.Error != null);
            foreach (var erroredResult in erroredResults)
            {
                throw new InfluxDataApiException(HttpStatusCode.BadRequest, erroredResult.Error);
            }

            return queryResponse;
        }
    }
}