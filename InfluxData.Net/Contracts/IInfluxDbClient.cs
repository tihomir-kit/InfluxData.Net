using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Client;
using InfluxData.Net.Infrastructure.Influx;
using InfluxData.Net.Models;
using InfluxData.Net.Enums;

namespace InfluxData.Net.Contracts
{
    internal interface IInfluxDbClient
    {
        #region Database Management

        /// <summary>Creates the database.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="database">The database.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> CreateDatabase(string dbName);

        /// <summary>Drops the database.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> DropDatabase(string dbName);

        /// <summary>Queries the list of databases.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> ShowDatabases();

        #endregion Database Management

        #region Basic Querying

        /// <summary>Writes the request to the endpoint.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="request">The request.</param>
        /// <param name="timePrecision">The time precision.</param>
        /// <returns></returns>
        Task<InfluxDbApiWriteResponse> Write(WriteRequest request, string timePrecision);

        /// <summary>Queries the endpoint.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="dbName">The name.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> Query(string name, string query);

        #endregion Basic Querying

        #region Continuous Queries

        // TODO: perhaps extract params into CreateCqRequest
        Task<InfluxDbApiResponse> CreateContinuousQuery(CqRequest cqRequest);

        Task<InfluxDbApiResponse> GetContinuousQueries(string dbName);

        Task<InfluxDbApiResponse> DeleteContinuousQuery(string dbName, string cqName);

        #endregion Continuous Queries

        #region Series

        Task<InfluxDbApiResponse> DropSeries(string dbName, string name);

        #endregion Series

        #region Clustering

        Task<InfluxDbApiResponse> CreateClusterAdmin(User user);

        Task<InfluxDbApiResponse> DeleteClusterAdmin(string name);

        Task<InfluxDbApiResponse> DescribeClusterAdmins();

        Task<InfluxDbApiResponse> UpdateClusterAdmin(User user, string name);

        #endregion Clustering

        #region Sharding

        Task<InfluxDbApiResponse> GetShardSpaces();

        Task<InfluxDbApiResponse> DropShardSpace(string dbName, string name);

        Task<InfluxDbApiResponse> CreateShardSpace(string dbName, ShardSpace shardSpace);

        #endregion Sharding

        #region Users

        Task<InfluxDbApiResponse> CreateDatabaseUser(string dbName, User user);

        Task<InfluxDbApiResponse> DeleteDatabaseUser(string dbName, string name);

        Task<InfluxDbApiResponse> DescribeDatabaseUsers(string dbName);

        Task<InfluxDbApiResponse> UpdateDatabaseUser(string dbName, User user, string name);

        Task<InfluxDbApiResponse> AuthenticateDatabaseUser(string dbName, string user, string password);

        #endregion Users

        #region Other

        Task<InfluxDbApiResponse> Ping();

        Task<InfluxDbApiResponse> ForceRaftCompaction();

        Task<InfluxDbApiResponse> Interfaces();

        Task<InfluxDbApiResponse> Sync();

        Task<InfluxDbApiResponse> ListServers();

        Task<InfluxDbApiResponse> RemoveServers(int id);

        /// <summary>Alters the retention policy.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="policyName">Name of the policy.</param>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="replication">The replication factor.</param>
        /// <returns><see cref="Task{TResult}"/></returns>
        Task<InfluxDbApiResponse> AlterRetentionPolicy(string policyName, string dbName, string duration, int replication);

        IFormatter GetFormatter();

        InfluxVersion GetVersion();

        #endregion Other
    }
}