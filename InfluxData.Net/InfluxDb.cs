using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfluxData.Net.Client;
using InfluxData.Net.Contracts;
using InfluxData.Net.Enums;
using InfluxData.Net.Helpers;
using InfluxData.Net.Infrastructure.Configuration;
using InfluxData.Net.Infrastructure.Influx;
using InfluxData.Net.Infrastructure.Validation;
using InfluxData.Net.Models;

namespace InfluxData.Net
{
    public class InfluxDb : IInfluxDb
    {
        internal readonly IEnumerable<ApiResponseErrorHandlingDelegate> NoErrorHandlers = Enumerable.Empty<ApiResponseErrorHandlingDelegate>();

        private readonly IInfluxDbClient _influxDbClient;

        public InfluxDb(string url, string username, string password, InfluxVersion influxVersion)
             : this(new InfluxDbClientConfiguration(new Uri(url), username, password, influxVersion))
        {
            Validate.NotNullOrEmpty(url, "The URL may not be null or empty.");
            Validate.NotNullOrEmpty(username, "The username may not be null or empty.");
        }

        internal InfluxDb(InfluxDbClientConfiguration influxDbClientConfiguration)
        {
            switch (influxDbClientConfiguration.InfluxVersion)
            {
                case InfluxVersion.v09x:
                    _influxDbClient = new InfluxDbClientBase(influxDbClientConfiguration);
                    break;
                case InfluxVersion.v096:
                    _influxDbClient = new InfluxDbClientV096(influxDbClientConfiguration);
                    break;
                case InfluxVersion.v095:
                    _influxDbClient = new InfluxDbClientV095(influxDbClientConfiguration);
                    break;
                case InfluxVersion.v092:
                    _influxDbClient = new InfluxDbClientV092(influxDbClientConfiguration);
                    break;
                case InfluxVersion.v08x:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException("influxDbClientConfiguration", String.Format("Unknown version {0}.", influxDbClientConfiguration));
            }
        }

        #region Database

        public async Task<InfluxDbApiResponse> CreateDatabaseAsync(string dbName)
        {
            return await _influxDbClient.CreateDatabase(dbName);
        }

        public async Task<InfluxDbApiResponse> DropDatabaseAsync(string dbName)
        {
            return await _influxDbClient.DropDatabase(dbName);
        }

        public async Task<List<Database>> ShowDatabasesAsync()
        {
            var response = await _influxDbClient.ShowDatabases();
            var queryResult = response.ReadAs<QueryResult>();
            var serie = queryResult.Results.Single().Series.Single();
            var databases = new List<Database>();

            foreach (var value in serie.Values)
            {
                databases.Add(new Database
                {
                    Name = (string)value[0]
                });
            }

            return databases;
        }

        #endregion Database

        #region Basic Querying

        public async Task<InfluxDbApiWriteResponse> WriteAsync(string database, Point point, string retenionPolicy = "default")
        {
            return await WriteAsync(database, new[] { point }, retenionPolicy);
        }

        public async Task<InfluxDbApiWriteResponse> WriteAsync(string database, Point[] points, string retenionPolicy = "default")
        {
            var request = new WriteRequest(_influxDbClient.GetFormatter())
            {
                Database = database,
                Points = points,
                RetentionPolicy = retenionPolicy
            };

            // TODO: handle precision (if set by client, it makes not difference because it gets overriden here)
            var result = await _influxDbClient.Write(request, ToTimePrecision(TimeUnit.Milliseconds));

            return result;
        }

        public async Task<List<Serie>> QueryAsync(string database, string query)
        {
            InfluxDbApiResponse response = await _influxDbClient.Query(database, query);
            var queryResult = response.ReadAs<QueryResult>();

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

        public async Task<InfluxDbApiResponse> CreateContinuousQueryAsync(CqRequest cqRequest)
        {
            return await _influxDbClient.CreateContinuousQuery(cqRequest);
        }

        public async Task<Serie> GetContinuousQueriesAsync(string dbName)
        {
            InfluxDbApiResponse response = await _influxDbClient.GetContinuousQueries(dbName);
            var queryResult = response.ReadAs<QueryResult>();//.Results.Single().Series;

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
            return await _influxDbClient.DeleteContinuousQuery(dbName, cqName);
        }

        #endregion Continuous Queries

        #region Series

        public async Task<InfluxDbApiResponse> DropSeriesAsync(string database, string serieName)
        {
            return await _influxDbClient.DropSeries(database, serieName);
        }

        #endregion Series

        #region Clustering

        public async Task<InfluxDbApiResponse> CreateClusterAdminAsync(string username, string adminPassword)
        {
            var user = new User { Name = username, Password = adminPassword };
            return await _influxDbClient.CreateClusterAdmin(user);
        }

        public async Task<InfluxDbApiResponse> DeleteClusterAdminAsync(string username)
        {
            return await _influxDbClient.DeleteClusterAdmin(username);
        }

        public async Task<List<User>> DescribeClusterAdminsAsync()
        {
            InfluxDbApiResponse response = await _influxDbClient.DescribeClusterAdmins();

            return response.ReadAs<List<User>>();
        }

        public async Task<InfluxDbApiResponse> UpdateClusterAdminAsync(string username, string password)
        {
            var user = new User { Name = username, Password = password };

            return await _influxDbClient.UpdateClusterAdmin(user, username);
        }

        #endregion Clustering

        #region Sharding

        public async Task<InfluxDbApiResponse> CreateShardSpaceAsync(string database, ShardSpace shardSpace)
        {
            return await _influxDbClient.CreateShardSpace(database, shardSpace);
        }

        public async Task<List<ShardSpace>> GetShardSpacesAsync()
        {
            InfluxDbApiResponse response = await _influxDbClient.GetShardSpaces();

            return response.ReadAs<List<ShardSpace>>();
        }

        public async Task<InfluxDbApiResponse> DropShardSpaceAsync(string database, string name)
        {
            return await _influxDbClient.DropShardSpace(database, name);
        }

        #endregion Sharding

        #region Users

        public async Task<InfluxDbApiResponse> CreateDatabaseUserAsync(string database, string name, string password, params string[] permissions)
        {
            var user = new User { Name = name, Password = password };
            user.SetPermissions(permissions);
            return await _influxDbClient.CreateDatabaseUser(database, user);
        }

        public async Task<InfluxDbApiResponse> DeleteDatabaseUserAsync(string database, string name)
        {
            return await _influxDbClient.DeleteDatabaseUser(database, name);
        }

        public async Task<List<User>> DescribeDatabaseUsersAsync(string database)
        {
            InfluxDbApiResponse response = await _influxDbClient.DescribeDatabaseUsers(database);

            return response.ReadAs<List<User>>();
        }

        public async Task<InfluxDbApiResponse> UpdateDatabaseUserAsync(string database, string name, string password, params string[] permissions)
        {
            var user = new User { Name = name, Password = password };
            user.SetPermissions(permissions);
            return await _influxDbClient.UpdateDatabaseUser(database, user, name);
        }

        public async Task<InfluxDbApiResponse> AlterDatabasePrivilegeAsync(string database, string name, bool isAdmin, params string[] permissions)
        {
            var user = new User { Name = name, IsAdmin = isAdmin };
            user.SetPermissions(permissions);
            return await _influxDbClient.UpdateDatabaseUser(database, user, name);
        }

        public async Task<InfluxDbApiResponse> AuthenticateDatabaseUserAsync(string database, string user, string password)
        {
            return await _influxDbClient.AuthenticateDatabaseUser(database, user, password);
        }

        #endregion Users

        #region Other

        public async Task<Pong> PingAsync()
        {
            var watch = Stopwatch.StartNew();

            var response = await _influxDbClient.Ping();

            watch.Stop();

            return new Pong
            {
                Version = response.Body,
                ResponseTime = watch.Elapsed,
                Success = true
            };
        }

        public async Task<InfluxDbApiResponse> ForceRaftCompactionAsync()
        {
            return await _influxDbClient.ForceRaftCompaction();
        }

        public async Task<List<string>> InterfacesAsync()
        {
            InfluxDbApiResponse response = await _influxDbClient.Interfaces();

            return response.ReadAs<List<string>>();
        }

        public async Task<bool> SyncAsync()
        {
            InfluxDbApiResponse response = await _influxDbClient.Sync();

            return response.ReadAs<bool>();
        }

        public async Task<List<Server>> ListServersAsync()
        {
            InfluxDbApiResponse response = await _influxDbClient.ListServers();

            return response.ReadAs<List<Server>>();
        }

        public async Task<InfluxDbApiResponse> RemoveServersAsync(int id)
        {
            return await _influxDbClient.RemoveServers(id);
        }

        public IFormatter GetFormatter()
        {
            return _influxDbClient.GetFormatter();
        }

        public InfluxVersion GetClientVersion()
        {
            return _influxDbClient.GetVersion();
        }

        #endregion Other

        #region Helpers

        public IInfluxDb SetLogLevel(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.None:

                    break;
                case LogLevel.Basic:

                    break;
                case LogLevel.Headers:

                    break;
                case LogLevel.Full:
                    break;
            }

            return this;
        }

        private string ToTimePrecision(TimeUnit timeUnit)
        {
            switch (timeUnit)
            {
                case TimeUnit.Seconds:
                    return "s";
                case TimeUnit.Milliseconds:
                    return "ms";
                case TimeUnit.Microseconds:
                    return "u";
                default:
                    throw new ArgumentException("time precision must be " + TimeUnit.Seconds + ", " + TimeUnit.Milliseconds + " or " + TimeUnit.Microseconds);
            }
        }

        #endregion Helpers
    }
}