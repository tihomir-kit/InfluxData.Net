using System.Threading.Tasks;
using InfluxData.Net.InfluxDb.Infrastructure;
using System.Collections.Generic;

namespace InfluxData.Net.InfluxDb.RequestClients.Modules
{
    public interface ISerieRequestModule
    {
        /// <summary>
        /// Gets distinct series.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="measurementName">Measurement name (optional).</param>
        /// <param name="filters">List of "WHERE" clause filters (optional).</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> GetSeries(string dbName, string measurementName, IList<string> filters = null);

        /// <summary>
        /// Deletes all data points from a serie.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="measurementName">Measurement name.</param>
        /// <param name="filters">List of "WHERE" clause filters (optional).</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> DropSeries(string dbName, string measurementName, IList<string> filters = null);

        /// <summary>
        /// Deletes all data points from multiple series.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="measurementName">A list of measurement names.</param>
        /// <param name="filters">List of "WHERE" clause filters (optional).</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> DropSeries(string dbName, IList<string> measurementNames, IList<string> filters = null);

        /// <summary>
        /// Gets distinct measurements.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="withClause">Regular expression with clause (optional).</param>
        /// <param name="filters">List of "WHERE" clause filters (optional).</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> GetMeasurements(string dbName, string withClause = null, IList<string> filters = null);

        /// <summary>
        /// Deletes all data points and series itself. Unlike DROP SERIES it also deletes
        /// the measurement from the index.
        /// </summary>
        /// <param name="dbName">Database name.</param>
        /// <param name="measurementName">Measurement name.</param>
        /// <returns></returns>
        Task<IInfluxDbApiResponse> DropMeasurement(string dbName, string measurementName);
    }
}