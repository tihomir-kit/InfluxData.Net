using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.Common.Enums;

namespace InfluxData.Net.InfluxDb.ClientSubModules
{
    /// <summary>
    /// A client sub-module which can then be shared by multiple threads/processes to be used
    /// for batch Point writing. It will keep the points in-memory for a specified interval. After the 
    /// interval times out, the collection will get dequeued and batch-written to influx. The BatchWriter
    /// will keep checking the collection after each interval until stopped.
    /// </summary>
    /// <see cref="http://www.codethinked.com/blockingcollection-and-iproducerconsumercollection"/>
    public interface IBatchWriter
    {
        /// <summary>
        /// Starts the batch writer.
        /// <param name="interval">Interval between writes (milliseconds).</param>
        /// </summary>
        void Start(int interval = 1000);

        /// <summary>
        /// Adds a single point to the BatchWriter points collection (uses BlockingCollection 
        /// internally for thread safety).
        /// </summary>
        /// <see cref="http://www.codethinked.com/blockingcollection-and-iproducerconsumercollection"/>
        /// <param name="point">Point to write.</param>
        void AddPoint(Point point);

        /// <summary>
        /// Adds multiple points to the BatchWriter points collection (uses BlockingCollection 
        /// internally for thread safety).
        /// </summary>
        /// <see cref="http://www.codethinked.com/blockingcollection-and-iproducerconsumercollection"/>
        /// <param name="points">Points to write.</param>
        void AddPoints(IEnumerable<Point> points);

        /// <summary>
        /// Stops the batch writer.
        /// </summary>
        void Stop();
    }

    internal interface IBatchWriterFactory : IBatchWriter
    {
        /// <summary>
        /// Creates a BatchWriter instance which can then be shared by multiple threads/processes to be used
        /// for batch Point writing. It will keep the points in-memory for a specified interval. After the 
        /// interval times out, the collection will get dequeued and batch-written to influx. The BatchWriter
        /// will keep checking the collection after each interval until stopped.
        /// </summary>
        /// <see cref="http://www.codethinked.com/blockingcollection-and-iproducerconsumercollection"/>
        /// <param name="dbName">Database name.</param>
        /// <param name="retenionPolicy">Retention policy.</param>
        /// <param name="precision">Precision.</param>
        /// <returns>BatchWriter instance.</returns>
        IBatchWriter CreateBatchWriter(string dbName, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds);
    }
}