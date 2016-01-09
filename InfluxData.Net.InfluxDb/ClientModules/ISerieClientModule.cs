using System.Collections.Generic;
using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using InfluxData.Net.InfluxDb.Models.Responses;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public interface ISerieClientModule
    {
        /// <summary>
        /// Gets distinct series.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="measurementName">Measurement name (optional).</param>
        /// <param name="filters">A collection of "WHERE" clause filters (optional).</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> GetSeriesAsync(string dbName, string measurementName, IEnumerable<string> filters = null);

        /// <summary>
        /// Deletes all data points from a serie.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="measurementName">Measurement name.</param>
        /// <param name="filters">A collection of "WHERE" clause filters (optional).</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> DropSeriesAsync(string dbName, string measurementName, IEnumerable<string> filters = null);

        /// <summary>
        /// Deletes all data points from multiple series.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="measurementName">A list of measurement names.</param>
        /// <param name="filters">A collection of "WHERE" clause filters (optional).</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> DropSeriesAsync(string dbName, IEnumerable<string> measurementNames, IEnumerable<string> filters = null);

        /// <summary>
        /// Gets distinct measurements.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="withClause">Regular expression with clause (optional).</param>
        /// <param name="filters">A collection of "WHERE" clause filters (optional).</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> GetMeasurementsAsync(string dbName, string withClause = null, IEnumerable<string> filters = null);

        /// <summary>
        /// Deletes all data points and series itself. Unlike DROP SERIES it also deletes
        /// the measurement from the index.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="measurementName">Measurement name.</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> DropMeasurementAsync(string dbName, string measurementName);
    }
}