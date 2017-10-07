using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxData.Helpers;
using InfluxData.Net.InfluxDb.Helpers;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.RequestClients;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class ClientModuleBase
    {
        protected IInfluxDbRequestClient RequestClient { get; private set; }

        protected IConfiguration Configuration { get; private set; }

        public ClientModuleBase(IInfluxDbRequestClient requestClient)
        {
            this.RequestClient = requestClient;
        }

        protected virtual async Task<IInfluxDataApiResponse> GetAndValidateQueryAsync(string query, string dbName = null, string epochFormat = null)
        {
            return await this.RequestAndValidateQueryAsync(query, HttpMethod.Get, dbName, epochFormat).ConfigureAwait(false);
        }

        protected virtual async Task<IInfluxDataApiResponse> PostAndValidateQueryAsync(string query, string dbName = null)
        {
            return await this.RequestAndValidateQueryAsync(query, HttpMethod.Post, dbName).ConfigureAwait(false);
        }

        protected virtual async Task<IInfluxDataApiResponse> RequestAndValidateQueryAsync(string query, HttpMethod method, string dbName = null, string epochFormat = null)
        {
            var response = await this.RequestClient.QueryAsync(query, method, dbName, epochFormat).ConfigureAwait(false);
            response.ValidateQueryResponse(this.RequestClient.Configuration.ThrowOnWarning);

            return response;
        }

        protected virtual async Task<IEnumerable<Serie>> ResolveSingleGetSeriesResultAsync(string query, string dbName = null, string epochFormat = null, long? chunkSize = null)
        {
            var response = await this.RequestClient.GetQueryAsync(query, dbName, epochFormat, chunkSize).ConfigureAwait(false);

            if (chunkSize == null)
                return this.ResolveSingleGetSeriesResult(response);
            else
                return this.ResolveSingleGetSeriesResultChunked(response);
        }

        protected virtual async Task<IEnumerable<SeriesResult>> ResolveGetSeriesResultAsync(string query, string dbName = null, string epochFormat = null, long? chunkSize = null)
        {
            var response = await this.RequestClient.GetQueryAsync(query, dbName, epochFormat, chunkSize).ConfigureAwait(false);

            if (chunkSize == null)
                return this.ResolveGetSeriesResult(response);
            else
                return this.ResolveGetSeriesResultChunked(response);
        }

        protected virtual IEnumerable<Serie> ResolveSingleGetSeriesResult(IInfluxDataApiResponse response)
        {
            var queryResponse = response.ReadAs<QueryResponse>().Validate(this.RequestClient.Configuration.ThrowOnWarning);
            var result = queryResponse.Results.Single();
            Validate.IsNotNull(result, "result");

            var series = result.Series != null ? result.Series.ToList() : new List<Serie>();

            return series;
        }

        protected virtual IEnumerable<Serie> ResolveSingleGetSeriesResultChunked(IInfluxDataApiResponse response)
        {
            string[] responseBodies = SplitChunkedResponse(response);
            var series = new List<Serie>();

            foreach (var responseBody in responseBodies)
            {
                var queryResponse = responseBody.ReadAs<QueryResponse>().Validate(this.RequestClient.Configuration.ThrowOnWarning);
                var result = queryResponse.Results.Single();
                Validate.IsNotNull(result, "result");

                if (result.Series != null)
                {
                    series.AddRange(result.Series.ToList());
                }
            }

            return series;
        }

        protected virtual IEnumerable<SeriesResult> ResolveGetSeriesResult(IInfluxDataApiResponse response)
        {
            return response.ReadAs<QueryResponse>().Validate(this.RequestClient.Configuration.ThrowOnWarning).Results;
        }

        protected virtual IEnumerable<SeriesResult> ResolveGetSeriesResultChunked(IInfluxDataApiResponse response)
        {
            string[] responseBodies = SplitChunkedResponse(response);
            var results = new List<SeriesResult>();

            foreach (var responseBody in responseBodies)
            {
                var queryResponse = responseBody.ReadAs<QueryResponse>().Validate(this.RequestClient.Configuration.ThrowOnWarning);

                if (queryResponse.Results != null)
                {
                    results.AddRange(queryResponse.Results);
                }
            }

            return results;
        }

        protected virtual string[] SplitChunkedResponse(IInfluxDataApiResponse response)
        {
            //Split response body for individual chunks
            return response.Body.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
