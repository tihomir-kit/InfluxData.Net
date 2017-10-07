using System.Collections.Generic;
using InfluxData.Net.InfluxDb.Models;
using System;
using InfluxData.Net.Common.Constants;

namespace InfluxData.Net.InfluxDb.ClientSubModules
{
    /// <summary>
    /// A client sub-module which can then be shared by multiple threads/processes to be used
    /// for batch Point writing in intervals (for example every five seconds). It will keep 
    /// the points in-memory for a specified interval. After the interval times out, the collection 
    /// will get dequeued and "batch-written" to influx. The BatchWriter will keep checking the
    /// collection for new points after each interval times out until stopped.
    /// </summary>
    /// <see cref="http://www.codethinked.com/blockingcollection-and-iproducerconsumercollection"/>
    public interface IBatchWriter
    {
        /// <summary>
        /// Starts the batch writer.
        /// <param name="interval">Interval between writes (milliseconds).</param>
        /// <param param name="continueOnError">Should continue running on write error? (defaults to false)</param>
        /// <param name="maxPointsPerBatch">Max batch point count (long max by default)</param>
        /// </summary>
        void Start(int interval = 1000, bool continueOnError = false, long maxPointsPerBatch = long.MaxValue);

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

        /// <summary>
        /// Sets the maximum size (point count) of a batch to commit to InfluxDB. If the collection currently 
        /// holds more than the `pointCount` points, any overflow will be commited in future requests on FIFO principle.
        /// </summary>
        /// <param name="pointCount">Max batch point count (long max by default).</param>
        void SetMaxBatchSize(long pointCount);

        /// <summary>
        /// On batch writing error event handler.
        /// </summary>
        event EventHandler<Exception> OnError;
    }

    internal interface IBatchWriterFactory : IBatchWriter
    {
        /// <summary>
        /// Creates a BatchWriter instance which can then be shared by multiple threads/processes to be used
        /// for batch Point writing in intervals (for example every five seconds) (for example every five seconds). 
        /// It will keep the points in-memory for a specified interval. After the interval times out, the collection 
        /// will get dequeued and "batch-written" to influx. The BatchWriter will keep checking the collection 
        /// for new points after each interval times out until stopped.
        /// </summary>
        /// <see cref="http://www.codethinked.com/blockingcollection-and-iproducerconsumercollection"/>
        /// <param name="dbName">Database name.</param>
        /// <param name="retenionPolicy">Retention policy.</param>
        /// <param name="precision">Precision.</param>
        /// <returns>BatchWriter instance.</returns>
        IBatchWriter CreateBatchWriter(string dbName, string retenionPolicy = null, string precision = TimeUnit.Milliseconds);
    }
}