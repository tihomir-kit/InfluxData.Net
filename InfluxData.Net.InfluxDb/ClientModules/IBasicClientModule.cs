﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.Common.Infrastructure;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public interface IBasicClientModule
    {
        /// <summary>
        /// Writes a single serie point to the database.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="point">A serie <see cref="{Point}" />.</param>
        /// <param name="retentionPolicy">The retention policy.</param>
        /// <param name="precision">InfluxDb time precision to use (defaults to 'ms')</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> WriteAsync(string dbName, Point point, string retentionPolicy = null, TimeUnit precision = TimeUnit.Milliseconds);

        /// <summary>
        /// Writes multiple serie points to the database.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="points">A serie <see cref="Array" />.</param>
        /// <param name="retentionPolicy">The retention policy.</param>
        /// <param name="precision">InfluxDb time precision to use (defaults to 'ms')</param>
        /// <returns></returns>
        Task<IInfluxDataApiResponse> WriteAsync(string dbName, IEnumerable<Point> points, string retentionPolicy = null, TimeUnit precision = TimeUnit.Milliseconds);

        /// <summary>
        /// Executes a query against the database.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="query">Query to execute.</param>
        /// <returns></returns>
        Task<IEnumerable<Serie>> QueryAsync(string dbName, string query);

        /// <summary>
        /// Executes multiple queries against the database in a single request and extracts and flattens
        /// the series from all results into a single <see cref="Serie"/> collection.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="queries">Queries to execute.</param>
        /// <returns></returns>
        Task<IEnumerable<Serie>> QueryAsync(string dbName, IEnumerable<string> queries);

        /// <summary>
        /// Executes multiple queries against the database in a single request.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="queries">Queries to execute.</param>
        /// <returns></returns>
        Task<IEnumerable<IEnumerable<Serie>>> MultiQueryAsync(string dbName, IEnumerable<string> queries);
    }
}