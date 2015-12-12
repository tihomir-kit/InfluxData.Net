using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Infrastructure.Influx;
using InfluxData.Net.Models;
using InfluxData.Net.Enums;

namespace InfluxData.Net.Contracts
{
    public interface IInfluxDb
    {
        // TODO: check returns from queries and implement proper replies (same ones that come from the influxDb)
        #region Database

        /// <summary>
        /// Create a new Database.
        /// </summary>
        /// <param name="dbName">The name of the new database</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> CreateDatabaseAsync(string dbName);

        /// <summary>
        /// Drop a database.
        /// </summary>
        /// <param name="dbName">The name of the database to delete.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> DropDatabaseAsync(string dbName);

        /// <summary>
        /// Describe all available databases.
        /// </summary>
        /// <returns>A list of all Databases</returns>
        Task<List<Database>> ShowDatabasesAsync();

        #endregion Database

        #region Basic Querying

        /// <summary>Write a single serie to the given database.</summary>
        /// <param name="database">The name of the database to write to.</param>
        /// <param name="point">A serie <see cref="{Point}" />.</param>
        /// <param name="retenionPolicy">The retenion policy.</param>
        /// <returns>TODO: comment</returns>
        Task<InfluxDbApiWriteResponse> WriteAsync(string database, Point point, string retenionPolicy = "default");

        /// <summary>Write multiple serie points to the given database.</summary>
        /// <param name="database">The name of the database to write to.</param>
        /// <param name="points">A serie <see cref="Array{Point}" />.</param>
        /// <param name="retenionPolicy">The retenion policy.</param>
        /// <returns>TODO: comment</returns>
        Task<InfluxDbApiWriteResponse> WriteAsync(string database, Point[] points, string retenionPolicy = "default");

        /// <summary>Execute a query agains a database.</summary>
        /// <param name="database">The name of the database.</param>
        /// <param name="query">The query to execute. For language specification please see
        /// <a href="https://influxdb.com/docs/v0.9/concepts/reading_and_writing_data.html">InfluxDb documentation</a>.</param>
        /// <returns>A list of Series which matched the query.</returns>
        /// <exception cref="InfluxDbApiException"></exception>
        Task<List<Serie>> QueryAsync(string database, string query);

        #endregion Basic Querying

        #region Continuous Queries

        // TODO: perhaps extract params into CreateCqRequest
        Task<InfluxDbApiResponse> CreateContinuousQueryAsync(CqRequest cqRequest);

        /// <summary>
        /// Describe all contious queries in a database.
        /// </summary>
        /// <param name="dbName">The name of the database for which all continuous queries should be described.</param>
        /// <returns>A list of all contious queries.</returns>
        Task<Serie> GetContinuousQueriesAsync(string dbName);

        /// <summary>
        /// Delete a continous query.
        /// </summary>
        /// <param name="dbName">The name of the database for which this query should be deleted.</param>
        /// <param name="cqName">The id of the query.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> DeleteContinuousQueryAsync(string dbName, string cqName);

        #endregion Continuous Queries

        #region Series

        /// <summary>
        /// Delete a serie.
        /// </summary>
        /// <param name="database">The database in which the given serie should be deleted.</param>
        /// <param name="serieName">The name of the serie.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> DropSeriesAsync(string database, string serieName);

        #endregion Series

        #region Clustering

        /// <summary>
        /// Create a new cluster admin.
        /// </summary>
        /// <param name="username">The name of the new admin.</param>
        /// <param name="adminPassword">The password for the new admin.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> CreateClusterAdminAsync(string username, string adminPassword);

        /// <summary>
        /// Delete a cluster admin.
        /// </summary>
        /// <param name="username">The name of the admin to delete.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> DeleteClusterAdminAsync(string username);

        /// <summary>
        /// Describe all cluster admins.
        /// </summary>
        /// <returns>A list of all admins.</returns>
        Task<List<User>> DescribeClusterAdminsAsync();

        /// <summary>
        /// Update the password of the given admin.
        /// </summary>
        /// <param name="username">The name of the admin for which the password should be updated.</param>
        /// <param name="password">The new password for the given admin.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> UpdateClusterAdminAsync(string username, string password);

        #endregion Clustering

        #region Sharding

        /// <summary>
        /// Create a ShardSpace in a Database.
        /// </summary>
        /// <param name="database">The name of the database.</param>
        /// <param name="shardSpace">The shardSpace to create in this database</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> CreateShardSpaceAsync(string database, ShardSpace shardSpace);

        /// <summary>
        /// Describe all existing shardspaces.
        /// </summary>
        /// <returns>A list of all <see cref="ShardSpace"></see>'s.</returns>
        Task<List<ShardSpace>> GetShardSpacesAsync();

        /// <summary>
        /// Drop a existing ShardSpace from a Database.
        /// </summary>
        /// <param name="database">The name of the database.</param>
        /// <param name="name">The name of the ShardSpace to delete.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> DropShardSpaceAsync(string database, string name);

        #endregion Sharding

        #region Users

        /// <summary>
        /// Create a new regular database user. Without any given permissions the new user is allowed to
        /// read and write to the database. The permission must be specified in regex which will match
        /// for the series. You have to specify either no permissions or both (readFrom and writeTo) permissions.
        /// </summary>
        /// <param name="database">The name of the database where this user is allowed.</param>
        /// <param name="name">The name of the new database user.</param>
        /// <param name="password">The password for this user.</param>
        /// <param name="permissions">An array of readFrom and writeTo permissions (in this order) and given in regex form.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> CreateDatabaseUserAsync(string database, string name, string password, params string[] permissions);

        /// <summary>
        /// Delete a database user.
        /// </summary>
        /// <param name="database">The name of the database the given user should be removed from.</param>
        /// <param name="name">The name of the user to remove.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> DeleteDatabaseUserAsync(string database, string name);

        /// <summary>
        /// Describe all database users allowed to acces the given database.
        /// </summary>
        /// <param name="database">The name of the database for which all users should be described.</param>
        /// <returns>A list of all users.</returns>
        Task<List<User>> DescribeDatabaseUsersAsync(string database);

        /// <summary>
        /// Update the password and/or the permissions of a database user.
        /// </summary>
        /// <param name="database">The name of the database where this user is allowed.</param>
        /// <param name="name">The name of the existing database user.</param>
        /// <param name="password">The password for this user.</param>
        /// <param name="permissions">An array of readFrom and writeTo permissions (in this order) and given in regex form.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> UpdateDatabaseUserAsync(string database, string name, string password, params string[] permissions);

        /// <summary>
        /// Alter the admin privilege of a given database user.
        /// </summary>
        /// <param name="database">The name of the database where this user is allowed.</param>
        /// <param name="name">The name of the existing database user.</param>
        /// <param name="isAdmin">If set to true this user is a database admin, otherwise it isnt.</param>
        /// <param name="permissions">An array of readFrom and writeTo permissions (in this order) and given in regex form.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> AlterDatabasePrivilegeAsync(string database, string name, bool isAdmin, params string[] permissions);

        /// <summary>
        /// Authenticate with the given credentials against the database.
        /// </summary>
        /// <param name="database">The name of the database where this user is allowed.</param>
        /// <param name="user">The name of the existing database user.</param>
        /// <param name="password">The password for this user.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> AuthenticateDatabaseUserAsync(string database, string user, string password);

        #endregion Users

        #region Other

        /// <summary>
        /// Ping this InfluxDB.
        /// </summary>
        /// <returns>The response of the ping execution.</returns>
        Task<Pong> PingAsync();

        /// <summary>
        /// Force Database compaction.
        /// </summary>
        /// <returns></returns>
        Task<InfluxDbApiResponse> ForceRaftCompactionAsync();

        /// <summary>
        /// List all interfaces influxDB is listening.
        /// </summary>
        /// <returns>A list of interface names.</returns>
        Task<List<string>> InterfacesAsync();

        /// <summary>
        /// Sync the database to the filesystem.
        /// </summary>
        /// <returns>true|false if successful.</returns>
        Task<bool> SyncAsync();

        /// <summary>
        /// List all servers which are member of the cluster.
        /// </summary>
        /// <returns>A list of all influxdb servers.</returns>
        Task<List<Server>> ListServersAsync();

        /// <summary>
        /// Remove the given Server from the cluster.
        /// </summary>
        /// <param name="id">The id of the server to remove.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> RemoveServersAsync(int id);

        IFormatter GetFormatter();

        InfluxVersion GetClientVersion();

        #endregion Other
    }
}