using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Infrastructure.Influx;
using InfluxData.Net.Models;
using InfluxData.Net.Enums;

namespace InfluxData.Net.Contracts
{
    // NOTE: potential "regions/classes": https://docs.influxdata.com/influxdb/v0.9/query_language/

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

        Task<InfluxDbApiResponse> DropSeries(string dbName, string serieName);

        #endregion Database Management

        #region Basic Querying

        /// <summary>Writes the request to the endpoint.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="writeRequest">The request.</param>
        /// <param name="timePrecision">The time precision.</param>
        /// <returns></returns>
        Task<InfluxDbApiWriteResponse> Write(WriteRequest writeRequest, string timePrecision);

        /// <summary>Queries the endpoint.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="dbName">The name.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        Task<InfluxDbApiResponse> Query(string dbName, string query);

        #endregion Basic Querying

        #region Continuous Queries

        Task<InfluxDbApiResponse> CreateContinuousQuery(ContinuousQuery cqRequest);

        Task<InfluxDbApiResponse> GetContinuousQueries(string dbName);

        Task<InfluxDbApiResponse> DeleteContinuousQuery(string dbName, string cqName);

        #endregion Continuous Queries

        #region Other

        Task<InfluxDbApiResponse> Ping();

        /// <summary>Alters the retention policy.</summary>
        /// <param name="errorHandlers">The error handlers.</param>
        /// <param name="policyName">Name of the policy.</param>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="replication">The replication factor.</param>
        /// <returns><see cref="Task{TResult}"/></returns>
        Task<InfluxDbApiResponse> AlterRetentionPolicy(string policyName, string dbName, string duration, int replication);

        #endregion Other

        #region Base

        IFormatter GetFormatter();

        InfluxVersion GetVersion();

        #endregion Base
    }
}