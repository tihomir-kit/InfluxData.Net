using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.Common.Helpers;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Clients;
using InfluxData.Net.InfluxDb.Clients.Modules;
using InfluxData.Net.InfluxDb.Models.Responses;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Formatters;

namespace InfluxData.Net.InfluxDb
{
    public class InfluxDb : IInfluxDb
    {
        private readonly IInfluxDbClient _influxDbClient;
        private readonly Lazy<IInfluxDbDatabaseModule> _influxDbDatabaseModule;
        private readonly Lazy<IInfluxDbBasicModule> _influxDbBasicModule;
        private readonly Lazy<IInfluxDbContinuousModule> _influxDbContinuousModule;

        public InfluxDb(string url, string username, string password, InfluxDbVersion influxVersion)
             : this(new InfluxDbClientConfiguration(new Uri(url), username, password, influxVersion))
        {
            Validate.NotNullOrEmpty(url, "The URL may not be null or empty.");
            Validate.NotNullOrEmpty(username, "The username may not be null or empty.");
        }

        internal InfluxDb(InfluxDbClientConfiguration influxDbClientConfiguration)
        {
            var clientFactory = new InfluxDbClientFactory(influxDbClientConfiguration);
            _influxDbClient = clientFactory.GetClient();

            _influxDbDatabaseModule = new Lazy<IInfluxDbDatabaseModule>(() => new InfluxDbDatabaseModule(_influxDbClient), true);
            _influxDbBasicModule = new Lazy<IInfluxDbBasicModule>(() => new InfluxDbBasicModule(_influxDbClient), true);
            _influxDbContinuousModule = new Lazy<IInfluxDbContinuousModule>(() => new InfluxDbContinuousModule(_influxDbClient), true);
        }

        #region Database

        public async Task<InfluxDbApiResponse> CreateDatabaseAsync(string dbName)
        {
            return await _influxDbDatabaseModule.Value.CreateDatabase(dbName);
        }

        public async Task<InfluxDbApiResponse> DropDatabaseAsync(string dbName)
        {
            return await _influxDbDatabaseModule.Value.DropDatabase(dbName);
        }

        public async Task<List<DatabaseResponse>> ShowDatabasesAsync()
        {
            var response = await _influxDbDatabaseModule.Value.ShowDatabases();
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
            return await _influxDbDatabaseModule.Value.DropSeries(dbName, serieName);
        }

        public async Task<InfluxDbApiResponse> AlterRetentionPolicy(string policyName, string dbName, string duration, int replication)
        {
            return await _influxDbDatabaseModule.Value.AlterRetentionPolicy(policyName, dbName, duration, replication);
        }

        #endregion Database

        #region Basic Querying

        public async Task<InfluxDbApiWriteResponse> WriteAsync(string dbName, Point point, string retenionPolicy = "default")
        {
            return await WriteAsync(dbName, new[] { point }, retenionPolicy);
        }

        public async Task<InfluxDbApiWriteResponse> WriteAsync(string dbName, Point[] points, string retenionPolicy = "default")
        {
            var request = new WriteRequest(_influxDbClient.GetFormatter())
            {
                Database = dbName,
                Points = points,
                RetentionPolicy = retenionPolicy
            };

            // TODO: handle precision (if set by client, it makes no difference because it gets overriden here)
            var result = await _influxDbBasicModule.Value.Write(request, TimeUnitUtility.ToTimePrecision(TimeUnit.Milliseconds));

            return result;
        }

        public async Task<List<Serie>> QueryAsync(string dbName, string query)
        {
            InfluxDbApiResponse response = await _influxDbBasicModule.Value.Query(dbName, query);
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
            return await _influxDbContinuousModule.Value.CreateContinuousQuery(continuousQuery);
        }

        public async Task<Serie> GetContinuousQueriesAsync(string dbName)
        {
            InfluxDbApiResponse response = await _influxDbContinuousModule.Value.GetContinuousQueries(dbName);
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
            return await _influxDbContinuousModule.Value.DeleteContinuousQuery(dbName, cqName);
        }

        public async Task<InfluxDbApiResponse> Backfill(string dbName, Backfill backfill)
        {
            return await _influxDbContinuousModule.Value.Backfill(dbName, backfill);
        }

        #endregion Continuous Queries

        #region Other

        public async Task<Pong> PingAsync()
        {
            var watch = Stopwatch.StartNew();

            var response = await _influxDbClient.PingAsync();

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
            return _influxDbClient.GetFormatter();
        }

        #endregion Other
    }
}