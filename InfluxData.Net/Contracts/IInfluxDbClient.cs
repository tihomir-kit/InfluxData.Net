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
        #region Database

        Task<InfluxDbApiResponse> CreateDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, Database database);

        Task<InfluxDbApiResponse> DropDatabase(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string name);

        Task<InfluxDbApiResponse> ShowDatabases(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        #endregion Database

        #region Basic Querying

        Task<InfluxDbApiWriteResponse> Write(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, WriteRequest request, string timePrecision);

        Task<InfluxDbApiResponse> Query(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string name, string query);

        #endregion Basic Querying

        #region Continuous Queries

        // TODO: perhaps extract params into CreateCqRequest
        Task<InfluxDbApiResponse> CreateContinuousQuery(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, CqRequest cqRequest);

        Task<InfluxDbApiResponse> GetContinuousQueries(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string dbName);

        Task<InfluxDbApiResponse> DeleteContinuousQuery(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string dbName, string cqName);

        #endregion Continuous Queries

        #region Series

        Task<InfluxDbApiResponse> DropSeries(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string dbName, string name);

        #endregion Series

        #region Clustering

        Task<InfluxDbApiResponse> CreateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, User user);

        Task<InfluxDbApiResponse> DeleteClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string name);

        Task<InfluxDbApiResponse> DescribeClusterAdmins(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiResponse> UpdateClusterAdmin(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, User user, string name);

        #endregion Clustering

        #region Sharding

        Task<InfluxDbApiResponse> GetShardSpaces(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiResponse> DropShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string dbName, string name);

        Task<InfluxDbApiResponse> CreateShardSpace(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string dbName, ShardSpace shardSpace);

        #endregion Sharding

        #region Users

        Task<InfluxDbApiResponse> CreateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string dbName, User user);

        Task<InfluxDbApiResponse> DeleteDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string dbName, string name);

        Task<InfluxDbApiResponse> DescribeDatabaseUsers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string dbName);

        Task<InfluxDbApiResponse> UpdateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string dbName, User user, string name);

        Task<InfluxDbApiResponse> AuthenticateDatabaseUser(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string dbName, string user, string password);

        #endregion Users

        #region Other

        Task<InfluxDbApiResponse> Ping(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiResponse> ForceRaftCompaction(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiResponse> Interfaces(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiResponse> Sync(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiResponse> ListServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers);

        Task<InfluxDbApiResponse> RemoveServers(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, int id);

        Task<InfluxDbApiResponse> AlterRetentionPolicy(IEnumerable<ApiResponseErrorHandlingDelegate> errorHandlers, string policyName, string dbName, string duration, int replication);

        IFormatter GetFormatter();

        InfluxVersion GetVersion();

        #endregion Other
    }
}