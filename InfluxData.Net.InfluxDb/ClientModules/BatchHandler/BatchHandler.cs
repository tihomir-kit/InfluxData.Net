using System.Collections.Generic;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb.Models;
//using System.Collections.Concurrent;

namespace InfluxData.Net.InfluxDb.ClientModules
{
    public class BatchHandler : IBatchHandler
    {
        private IBasicClientModule _basicClientModule;
        private string _dbName;
        private string _retentionPolicy;
        private TimeUnit _precision;

        /// <summary>
        /// Concurrent readings queue.
        /// <see cref="http://www.codethinked.com/blockingcollection-and-iproducerconsumercollection"/>
        /// </summary>
        //public BlockingCollection<Point> PointCollection { get; set; }

        internal BatchHandler(IBasicClientModule basicClientModule)
        {
            _basicClientModule = basicClientModule;
        }

        internal BatchHandler(IBasicClientModule basicClientModule, string dbName, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds)
        {
            _basicClientModule = basicClientModule;
            _dbName = dbName;
            _retentionPolicy = retenionPolicy;
            _precision = precision;
        }

        public virtual IBatchHandler CreateBatchHandler(string dbName, string retenionPolicy = "default", TimeUnit precision = TimeUnit.Milliseconds)
        {
            return new BatchHandler(_basicClientModule, dbName, retenionPolicy, precision);
        }

        public virtual void Start()
        {

        }

        public virtual void AddPoints(IEnumerable<Point> points)
        {

        }

        public virtual void Stop()
        {

        }
    }
}
