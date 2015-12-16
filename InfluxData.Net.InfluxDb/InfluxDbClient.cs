using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.RequestClients;
using InfluxData.Net.InfluxDb.RequestClients.Modules;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Formatters;

namespace InfluxData.Net.InfluxDb
{
    public class InfluxDbClient : IInfluxDbClient
    {
        private readonly IInfluxDbRequestClient _requestClient;
        private readonly Lazy<IBasicRequestModule> _basicRequestModule;
        private readonly Lazy<IDatabaseRequestModule> _databaseRequestModule;
        private readonly Lazy<ICqRequestModule> _cqRequestModule;

        public InfluxDbClient(string url, string username, string password, InfluxDbVersion influxVersion)
             : this(new InfluxDbClientConfiguration(new Uri(url), username, password, influxVersion))
        {
            Validate.NotNullOrEmpty(url, "The URL may not be null or empty.");
            Validate.NotNullOrEmpty(username, "The username may not be null or empty.");
        }

        internal InfluxDbClient(InfluxDbClientConfiguration configuration)
        {
            var requestClientFactory = new RequestFactory(configuration);
            _requestClient = requestClientFactory.GetRequestClient();

            _basicRequestModule = new Lazy<IBasicRequestModule>(() => new BasicRequestModule(_requestClient), true);
            _databaseRequestModule = new Lazy<IDatabaseRequestModule>(() => new InfluxDbDatabaseModule(_requestClient), true);
            _cqRequestModule = new Lazy<ICqRequestModule>(() => new CqRequestModule(_requestClient), true);
        }

        #region Database

        public async Task<InfluxDbApiResponse> CreateDatabaseAsync(string dbName)
        {
            return await _databaseRequestModule.Value.CreateDatabase(dbName);
        }

        public async Task<InfluxDbApiResponse> DropDatabaseAsync(string dbName)
        {
            return await _databaseRequestModule.Value.DropDatabase(dbName);
        }

        public async Task<List<DatabaseResponse>> ShowDatabasesAsync()
        {
            var response = await _databaseRequestModule.Value.ShowDatabases();
            var queryResult = response.ReadAs<QueryResponse>();
            var serie = queryResult.Results.Single().Series.Single();
            var databases = new List<DatabaseResponse>();

            foreach (var value in serie.Values)
            {
                databases.Add(new DatabaseResponse
                {
                    Name = (string)value[0]
                });
            }

            return databases;
        }

        public async Task<InfluxDbApiResponse> DropSeriesAsync(string dbName, string serieName)
        {
            return await _databaseRequestModule.Value.DropSeries(dbName, serieName);
        }

        public async Task<InfluxDbApiResponse> AlterRetentionPolicy(string policyName, string dbName, string duration, int replication)
        {
            return await _databaseRequestModule.Value.AlterRetentionPolicy(policyName, dbName, duration, replication);
        }

        #endregion Database

        #region Basic Querying

        public async Task<InfluxDbApiWriteResponse> WriteAsync(string dbName, Point point, string retenionPolicy = "default")
        {
            return await WriteAsync(dbName, new[] { point }, retenionPolicy);
        }

        public async Task<InfluxDbApiWriteResponse> WriteAsync(string dbName, Point[] points, string retenionPolicy = "default")
        {
            var request = new WriteRequest(_requestClient.GetFormatter())
            {
                Database = dbName,
                Points = points,
                RetentionPolicy = retenionPolicy
            };

            // TODO: handle precision (if set by client, it makes no difference because it gets overriden here)
            var result = await _basicRequestModule.Value.Write(request, TimeUnitUtility.ToTimePrecision(TimeUnit.Milliseconds));

            return result;
        }

        public async Task<List<Serie>> QueryAsync(string dbName, string query)
        {
            InfluxDbApiResponse response = await _basicRequestModule.Value.Query(dbName, query);
            var queryResult = response.ReadAs<QueryResponse>();

            Validate.NotNull(queryResult, "queryResult");
            Validate.NotNull(queryResult.Results, "queryResult.Results");

            // Apparently a 200 OK can return an error in the results
            // https://github.com/influxdb/influxdb/pull/1813
            var error = queryResult.Results.Single().Error;
            if (error != null)
            {
                throw new InfluxDbApiException(System.Net.HttpStatusCode.BadRequest, error);
            }

            var result = queryResult.Results.Single().Series;

            return result != null ? result.ToList() : new List<Serie>();
        }
        
        #endregion Basic Querying

        #region Continuous Queries

        public async Task<InfluxDbApiResponse> CreateContinuousQueryAsync(ContinuousQuery continuousQuery)
        {
            return await _cqRequestModule.Value.CreateContinuousQuery(continuousQuery);
        }

        public async Task<Serie> GetContinuousQueriesAsync(string dbName)
        {
            InfluxDbApiResponse response = await _cqRequestModule.Value.GetContinuousQueries(dbName);
            var queryResult = response.ReadAs<QueryResponse>();//.Results.Single().Series;

            Validate.NotNull(queryResult, "queryResult");
            Validate.NotNull(queryResult.Results, "queryResult.Results");

            // Apparently a 200 OK can return an error in the results
            // https://github.com/influxdb/influxdb/pull/1813
            var error = queryResult.Results.Single().Error;
            if (error != null)
            {
                throw new InfluxDbApiException(System.Net.HttpStatusCode.BadRequest, error);
            }

            var series = queryResult.Results.Single().Series;

            return series != null ? series.Where(p => p.Name == dbName).FirstOrDefault() : new Serie();
        }

        public async Task<InfluxDbApiResponse> DeleteContinuousQueryAsync(string dbName, string cqName)
        {
            return await _cqRequestModule.Value.DeleteContinuousQuery(dbName, cqName);
        }

        public async Task<InfluxDbApiResponse> Backfill(string dbName, Backfill backfill)
        {
            return await _cqRequestModule.Value.Backfill(dbName, backfill);
        }

        #endregion Continuous Queries

        #region Other

        public async Task<Pong> PingAsync()
        {
            var watch = Stopwatch.StartNew();

            var response = await _requestClient.PingAsync();

            watch.Stop();

            return new Pong
            {
                Version = response.Body,
                ResponseTime = watch.Elapsed,
                Success = true
            };
        }

        public IInfluxDbFormatter GetFormatter()
        {
            return _requestClient.GetFormatter();
        }

        #endregion Other
    }
}