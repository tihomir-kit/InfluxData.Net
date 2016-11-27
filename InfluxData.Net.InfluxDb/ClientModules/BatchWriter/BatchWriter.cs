using System.Collections.Generic;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Models;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class BatchWriter : IBatchWriterFactory
    {
        private IBasicClientModule _basicClientModule;
        private string _dbName;
        private int _interval;
        private string _retentionPolicy;
        private TimeUnit _precision;
        private bool _isRunning;

        /// <summary>
        /// Concurrent readings queue.
        /// <see cref="http://www.codethinked.com/blockingcollection-and-iproducerconsumercollection"/>
        /// </summary>
        private BlockingCollection<Point> _pointCollection;

        internal BatchWriter(IBasicClientModule basicClientModule)
        {
            _basicClientModule = basicClientModule;
        }

        internal BatchWriter(IBasicClientModule basicClientModule, string dbName, int interval, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds)
        {
            _basicClientModule = basicClientModule;
            _dbName = dbName;
            _interval = 1000;
            _retentionPolicy = retenionPolicy;
            _precision = precision;
        }

        public virtual IBatchWriter CreateBatchWriter(string dbName, int interval, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds)
        {
            if (interval <= 0)
                throw new ArgumentException("Interval must be a positive int value (milliseconds)");

            return new BatchWriter(_basicClientModule, dbName, interval, retenionPolicy, precision);
        }

        public virtual void Start()
        {
            _isRunning = true;
            this.EnqueueBatchWritingAsync();
        }

        public virtual void AddPoint(Point point)
        {
            _pointCollection.Add(point);
        }

        public virtual void AddPoints(IEnumerable<Point> points)
        {
            foreach (var point in points)
            {
                _pointCollection.Add(point);
            }
        }

        public virtual void Stop()
        {
            _isRunning = false;
        }

        private async Task EnqueueBatchWritingAsync()
        {
            if (!_isRunning)
                return;

            await Task.Delay(_interval); // TODO: should be configurable
            this.WriteBatchedPointsAsync();
            this.EnqueueBatchWritingAsync();
        }

        private async Task WriteBatchedPointsAsync()
        {
            var pointCount = _pointCollection.Count;
            IList<Point> points = new List<Point>();

            for (var i = 0; i < pointCount; i++)
            {
                Point point;
                var dequeueSuccess = _pointCollection.TryTake(out point);

                if (dequeueSuccess)
                {
                    points.Add(point);
                }
                else
                {
                    throw new Exception("Could not dequeue the collection");
                }
            }

            if (points.Count > 0)
            {
                await _basicClientModule.WriteAsync(_dbName, points, _retentionPolicy, _precision);
            }
        }
    }
}
